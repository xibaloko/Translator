using DynamicTranslator.Options;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DynamicTranslator.IoC
{
    public static class Extensions
    {
        public static IServiceCollection AddDynamicTranslator(this IServiceCollection services, Action<DynamicTranslatorOptions> configure)
        {
            services.Configure(configure);

            services.AddLocalization();

            return services;
        }
    }
}
