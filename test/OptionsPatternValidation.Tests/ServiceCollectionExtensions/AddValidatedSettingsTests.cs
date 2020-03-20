using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OptionsPatternValidation.Tests.TestHelpers;
using OptionsPatternValidation.Tests.TestSettings;
using OptionsPatternValidation.Tests.TestSettings.AttributeValidation;
using OptionsPatternValidation.Tests.TestSettingsJson;
using Xunit;

namespace OptionsPatternValidation.Tests.ServiceCollectionExtensions
{
    /// <summary>Test the DataAnnotations AddValidatedSettings(config) method.</summary>
    public class AddValidatedSettingsTests
    {
        [Fact]
        public void Wires_up_SimpleAttributeValidatedSettings_from_string()
        {
            var services = new ServiceCollection();

            
            const string json = @"
{
""Simple"": {
    ""IntegerA"": 76,
    ""BooleanB"": true
  }
}
                ";

            var configuration = ConfigurationTestBuilder.BuildFromJsonString(json);
            
            services.AddValidatedSettings<SimpleAttributeValidatedSettings>(configuration);

            var serviceProvider = services.BuildServiceProvider();
            var result = serviceProvider.GetRequiredService<IOptions<SimpleAttributeValidatedSettings>>().Value;

            Assert.NotNull(result);
            Assert.Equal(76, result.IntegerA);
        }
        
        [Fact]
        public void Wires_up_SimpleAttributeValidatedSettings_from_Test1_file()
        {
            var services = new ServiceCollection();

            var configuration = ConfigurationTestBuilder.BuildFromEmbeddedResource(JsonIndex.AddSettingsTest2);
            
            services.AddValidatedSettings<SimpleAttributeValidatedSettings>(configuration);

            var serviceProvider = services.BuildServiceProvider();
            var result = serviceProvider.GetRequiredService<IOptions<SimpleAttributeValidatedSettings>>().Value;

            Assert.NotNull(result);
            Assert.Equal(89, result.IntegerA);
        }
    }
}