using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OptionsPatternValidation.Extensions;

namespace OptionsPatternValidation
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Bind a section of the appsettings.json to an options pattern POCO without validation.</summary>
        public static IServiceCollection AddSettings<T>(
            this IServiceCollection services,
            IConfiguration configuration
            ) where T : class
        {
            var sectionName = SettingsSectionNameAttribute.GetSettingsSectionName<T>();
            
            var configurationSection = configuration.GetSection(sectionName);
            
            services.AddOptions<T>()
                .Bind(configurationSection);
            
            return services;
        }
        
        /// <summary>Bind a section of the appsettings.json to an options pattern POCO along
        /// with wiring up recursively-validated DataAnnotation validation.</summary>
        public static IServiceCollection AddValidatedSettings<T>(
            this IServiceCollection services,
            IConfiguration configuration
            ) where T : class
        {
            var sectionName = SettingsSectionNameAttribute.GetSettingsSectionName<T>();
            
            var configurationSection = configuration.GetSection(sectionName);
            
            services.AddOptions<T>()
                .Bind(configurationSection)
                .RecursivelyValidateDataAnnotations();
            
            return services;
        }
        
        /// <summary>Bind a section of the appsettings.json to a POCO along with wiring up
        /// IValidateOptions<T> validation.</summary>
        public static IServiceCollection AddValidatedSettings<T, TValidator>(
            this IServiceCollection services,
            IConfiguration configuration
            )
            where T : class, new()
            where TValidator : class, IValidateOptions<T>
        {
            var sectionName = SettingsSectionNameAttribute.GetSettingsSectionName<T>();

            var configurationSection = configuration.GetSection(sectionName);

            services.AddOptions<T>()
                .Bind(configurationSection);

            services.AddSingleton<IValidateOptions<T>, TValidator>();

            return services;
        }

    }
}