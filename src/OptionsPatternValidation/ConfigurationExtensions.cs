using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OptionsPatternValidation.ValidationOptions;

namespace OptionsPatternValidation
{
    /// <summary>Extension for <see cref="IConfiguration"/> to fetch and validate a section
    /// of the configuration into a strongly-typed object.  The fetched values are a
    /// snapshot of the configuration section at the time of the call.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>Fetch and validate a section of the current configuration values.</summary>
        /// <param name="configuration">The <see cref="IConfiguration"/> object.</param>
        /// <typeparam name="T">The type of the settings object to bind the configuration to.</typeparam>
        /// <exception cref="OptionsValidationException">The set of validation failures.</exception>
        public static T GetValidatedConfigurationSection<T>(
            this IConfiguration configuration
            ) where T : class, new()
        {
            var sectionName = SettingsSectionNameAttribute.GetSettingsSectionName<T>();
            var configurationSection = configuration.GetSection(sectionName);
            var settings = configurationSection.Get<T>() ?? new T();
            
            var validator = new RecursiveDataAnnotationValidateOptions<T>(null);
            var validateOptionsResult = validator.Validate(null, settings);
            if (validateOptionsResult.Failed)
                throw new OptionsValidationException(
                    $"Section: {sectionName}",
                    typeof(T),
                    validateOptionsResult.Failures
                );

            return settings;
        }
    }
}