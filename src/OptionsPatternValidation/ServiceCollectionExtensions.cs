using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OptionsPatternValidation.Extensions;
using OptionsPatternValidation.ValidationOptions;

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
        /// IValidateOptions validation.</summary>
        public static IServiceCollection AddValidatedSettings<T, TValidator>(
            this IServiceCollection services,
            IConfiguration configuration
            )
            where T : class
            where TValidator : class, IValidateOptions<T>
        {
            var sectionName = SettingsSectionNameAttribute.GetSettingsSectionName<T>();
            var configurationSection = configuration.GetSection(sectionName);

            services.AddOptions<T>()
                .Bind(configurationSection);

            services.AddSingleton<IValidateOptions<T>, TValidator>();

            return services;
        }

        /// <summary><para>Bind a section of the appsettings.json to an options pattern POCO along
        /// with wiring up recursively-validated DataAnnotation validation.  It will immediately
        /// validate the POCO after binding and throw an exception if validation fails.</para>
        /// <para>This method outputs a reference to the POCO, loaded with setting values
        /// at the time of registration.  This reference should be the same as the reference
        /// that you get back later from the IOptions.Value call.</para>
        /// <para>WARNING: This method is a experimental and it's likely that the IOptionsMonitor
        /// approach will *not* notify the application about changed configuration values.  So
        /// this method is not suitable for long-lived processes, but should work okay for apps
        /// that restart whenever configuration settings are changed.</para></summary>
        /// <exception cref="Microsoft.Extensions.Options.OptionsValidationException">
        /// Thrown when the validation of the POCO fails.</exception>
        public static IServiceCollection AddEagerlyValidatedSettings<T>(
            this IServiceCollection services,
            IConfiguration configuration,
            out T outputOptions
            ) where T : class, new()
        {
            var sectionName = SettingsSectionNameAttribute.GetSettingsSectionName<T>();
            var configurationSection = configuration.GetSection(sectionName);
            var settings = configurationSection.Get<T>();
            
            var options = Options.Create(settings);
            
            services.AddSingleton(options);
            services.AddSingleton<IValidateOptions<T>>(
                new RecursiveDataAnnotationValidateOptions<T>(
                    null
                ));

            var validator = new RecursiveDataAnnotationValidateOptions<T>(null);
            var validateOptionsResult = validator.Validate(null, options.Value);
            if (validateOptionsResult.Failed)
                throw new OptionsValidationException(
                    $"Section: {sectionName}",
                    typeof(T),
                    validateOptionsResult.Failures
                    );
            
            outputOptions = options.Value;
            
            return services;
        }
    }
}