using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using OptionsPatternValidation.Extensions;
using OptionsPatternValidation.Tests.TestHelpers;
using OptionsPatternValidation.Tests.TestSettings;
using OptionsPatternValidation.Tests.TestSettings.AttributeValidation;
using OptionsPatternValidation.Tests.TestSettingsJson;
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
            var configurationSection = configuration.GetSection(sectionName);

            var simpleSettings = new AttributeValidatedSettings();
            var options = Options.Create<AttributeValidatedSettings>(simpleSettings);
            var validatedSettings = options.Value;

            services.AddSingleton<IOptions<AttributeValidatedSettings>>(options);

            var serviceProvider = services.BuildServiceProvider();

            var result1 = validatedSettings;
            var result2 = serviceProvider.GetRequiredService<IOptions<AttributeValidatedSettings>>().Value;

            Assert.True(object.ReferenceEquals(result1, result2));
        }

        
    }
}