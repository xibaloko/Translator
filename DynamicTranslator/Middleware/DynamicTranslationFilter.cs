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
            _logger.LogError("Invalid culture specified in Accept-Language header: {Language}", language);

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
            var elementType = type.IsGenericType ? type.GetGenericArguments()[0] : typeof(object);
            var listType = typeof(List<>).MakeGenericType(elementType);
            var list = (IList)Activator.CreateInstance(listType);

            foreach (var item in (IEnumerable)instance)
            {
                var translated = TranslateObject(item, localizer);
                list.Add(translated);
            }

            return list;
        }

        if (type.GetCustomAttribute<TranslateAllAttribute>() != null)
            return TranslateAll(instance, localizer);
        else if (type.GetCustomAttribute<TranslateKeysAttribute>() != null)
            return TranslateKeys(instance, localizer);
        else
            return TranslateValues(instance, localizer);
    }

    private static object TranslateAll(object instance, IStringLocalizer localizer)
    {
        throw new NotImplementedException();
    }

    private static object TranslateKeys(object instance, IStringLocalizer localizer)
    {
        throw new NotImplementedException();
    }

    private object TranslateValues(object instance, IStringLocalizer localizer)
    {
        var type = instance.GetType();

        var copy = Activator.CreateInstance(type);

        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (prop.GetCustomAttribute<IgnoreTranslationAttribute>() != null || type.GetCustomAttribute<DisableTranslatorAttribute>() != null)
            {
                prop.SetValue(copy, prop.GetValue(instance));

                continue;
            }

            var value = prop.GetValue(instance);

            if (value is string strValue)
            {
                var translated = localizer[strValue].Value ?? strValue;
                prop.SetValue(copy, translated);
            }
            else if (value != null && (value is IEnumerable || value.GetType().IsClass))
            {
                var translated = TranslateObject(value, localizer);
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
