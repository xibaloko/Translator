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
    private readonly HashSet<object> _visited = new HashSet<object>();

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
            _logger.LogWarning("No culture specified in Accept-Language header.");
            await next();
            return;
        }

        try
        {
            CultureInfo.CurrentCulture = new CultureInfo(language);
            CultureInfo.CurrentUICulture = new CultureInfo(language);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Invalid culture specified in Accept-Language header: {Language}", language);
            await next();
            return;
        }

        if (context.Result is ObjectResult result && result.Value != null)
        {
            var localizer = _localizerFactory.Create(_resourceType.FullName, _resourceType.Assembly.GetName().Name);

            try
            {
                var translated = TranslateObject(result.Value, localizer);
                result.Value = translated;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to translate response object.");
            }
        }

        await next();
    }

    private object TranslateObject(object instance, IStringLocalizer localizer)
    {
        if (_visited.Contains(instance))
            return instance;

        _visited.Add(instance);

        var type = instance.GetType();

        if (IsDictionaryType(type))
        {
            _logger.LogWarning("Translation of dictionaries is not supported: {Type}", type.FullName);
            return instance;
        }

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

        return TranslateValues(instance, localizer);
    }

    private object TranslateValues(object instance, IStringLocalizer localizer)
    {
        var type = instance.GetType();

        var ctor = type.GetConstructor(Type.EmptyTypes);

        if (ctor == null)
        {
            _logger.LogWarning("No parameterless constructor for type {Type}, skipping translation.", type.FullName);
            return instance;
        }

        var copy = ctor.Invoke(null);

        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!prop.CanWrite || prop.SetMethod?.IsPublic != true)
                continue;

            if (prop.GetCustomAttribute<IgnoreTranslationAttribute>() != null ||
                type.GetCustomAttribute<DisableTranslatorAttribute>() != null)
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

    private static bool IsCollectionType(Type type) => typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string);

    private static bool IsDictionaryType(Type type) => typeof(IDictionary).IsAssignableFrom(type);
}
