using DynamicTranslator.Attributes;
using DynamicTranslator.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;

public class GenericTranslationFilter : IAsyncResultFilter 
{
    private readonly ILogger<GenericTranslationFilter> _logger;
    private readonly IStringLocalizerFactory _localizerFactory;
    private readonly Type _resourceType;

    public GenericTranslationFilter(
        ILogger<GenericTranslationFilter> logger,
        IStringLocalizerFactory localizerFactory,
        IOptions<DynamicTranslatorOptions> options)
    {
        _logger = logger;
        _localizerFactory = localizerFactory;
        _resourceType = options.Value.ResourceType;
    }

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var language = context.HttpContext.Request.Headers["Accept-Language"].ToString();

        if (string.IsNullOrWhiteSpace(language))
        {
            _logger.LogWarning("No culture specified");

            await next();
            return;
        }

        try
        {
            CultureInfo.CurrentCulture = new CultureInfo(language);
            CultureInfo.CurrentUICulture = new CultureInfo(language);
        }
        catch
        {
            _logger.LogWarning("Invalid culture specified in Accept-Language header: {Language}", language);
            
            await next();
            return;
        }

        if (context.Result is ObjectResult result && result.Value != null)
        {
            var localizer = _localizerFactory.Create(_resourceType.FullName, _resourceType.Assembly.GetName().Name);

            var translated = TranslateObject(result.Value, localizer);
            result.Value = translated;
        }

        await next();
    }

    private object TranslateObject(object instance, IStringLocalizer localizer)
    {
        var type = instance.GetType();

        if (IsCollectionType(type))
        {
            var list = new List<object>();

            foreach (var item in (IEnumerable)instance)
            {
                var translated = TranslateObject(item, localizer);
                list.Add(translated);
            }

            return list;
        }

        bool translateValues = type.GetCustomAttribute<TranslateAllAttribute>() != null;

        var copy = Activator.CreateInstance(type);

        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (prop.GetCustomAttribute<IgnoreTranslationAttribute>() != null)
            {
                prop.SetValue(copy, prop.GetValue(instance));

                continue;
            }

            var value = prop.GetValue(instance);

            if (value is string strValue && translateValues)
            {
                var translated = localizer[strValue].Value ?? strValue;
                prop.SetValue(copy, translated);
            }
            else
            {
                prop.SetValue(copy, value);
            }
        }

        return copy;
    }

    private static bool IsCollectionType(Type type)
    {
        if (type == typeof(string)) return false;

        return typeof(IEnumerable).IsAssignableFrom(type);
    }
}
