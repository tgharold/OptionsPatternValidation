using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OptionsPatternValidation.Extensions;
using OptionsPatternValidation.Tests.Helpers;
using OptionsPatternValidation.Tests.Json;
using OptionsPatternValidation.Tests.Settings.AttributeValidation;
using OptionsPatternValidation.ValidationOptions;
using Xunit;

namespace OptionsPatternValidation.Tests.Experiments
{
    /// <summary>Experiment with binding during Startup.ConfigureServices,
    /// immediately running the validation, and then returning a singleton. 
    /// </summary>
    public class EagerValidation1
    {
        [Theory]
        [InlineData(JsonIndex.AttributeValidated.Test2)]
        public void Test_reference_equals_1(string filename)
        {
            var services = new ServiceCollection();

            var configuration = ConfigurationTestBuilder.BuildFromEmbeddedResource(filename);
            var sectionName = SettingsSectionNameAttribute.GetSettingsSectionName<AttributeValidatedSettings>();
            var configurationSection = configuration.GetSection(sectionName);

            // This is the standard way, it results in lazy validation
            services.AddOptions<AttributeValidatedSettings>()
                .Bind(configurationSection)
                .RecursivelyValidateDataAnnotations();
            
            var serviceProvider = services.BuildServiceProvider();

            var result1 = serviceProvider.GetRequiredService<IOptions<AttributeValidatedSettings>>().Value;
            var result2 = serviceProvider.GetRequiredService<IOptions<AttributeValidatedSettings>>().Value;

            Assert.True(object.ReferenceEquals(result1, result2));
        }
        
        [Theory]
        [InlineData(JsonIndex.AttributeValidated.Test2)]
        public void Test_reference_equals_2(string filename)
        {
            var services = new ServiceCollection();

            var configuration = ConfigurationTestBuilder.BuildFromEmbeddedResource(filename);
            var sectionName = SettingsSectionNameAttribute.GetSettingsSectionName<AttributeValidatedSettings>();
            var simpleSettings = configuration.GetSection(sectionName).Get<AttributeValidatedSettings>();
            
            // This approach gives eager validation
            // Downside is that I think IOptionsMonitor<T> will not inform of changes
            
            var options = Options.Create<AttributeValidatedSettings>(simpleSettings);
            
            services.AddSingleton<IOptions<AttributeValidatedSettings>>(options);
            services.AddSingleton<IValidateOptions<AttributeValidatedSettings>>(
                new RecursiveDataAnnotationValidateOptions<AttributeValidatedSettings>(
                    null
                ));

            var serviceProvider = services.BuildServiceProvider();

            var validator = serviceProvider.GetRequiredService<IValidateOptions<AttributeValidatedSettings>>();
            var validateOptionsResult = validator.Validate(null, options.Value);
            if (validateOptionsResult.Failed)
                throw new Exception();
            
            var validatedSettings = options.Value;

            var result1 = validatedSettings;
            var result2 = serviceProvider.GetRequiredService<IOptions<AttributeValidatedSettings>>().Value;

            Assert.True(object.ReferenceEquals(result1, result2));
        }

        
    }
}