using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OptionsPatternValidation.Tests.TestHelpers;
using OptionsPatternValidation.Tests.TestSettings;
using OptionsPatternValidation.Tests.TestSettingsJson;
using Xunit;

namespace OptionsPatternValidation.Tests.ServiceCollectionExtensions
{
    /// <summary>Test the generic AddSettings(config) method.</summary>
    public class AddSettingsTests
    {
        [Fact]
        public void Wires_up_SimpleAttributeValidatedSettings_from_string()
        {
            var services = new ServiceCollection();

            
            const string json = @"
{
""Simple"": {
    ""IntegerA"": 1075
  }
}
                ";

            var configuration = ConfigurationTestBuilder.BuildFromJsonString(json);
            
            services.AddSettings<SimpleSettings>(configuration);

            var serviceProvider = services.BuildServiceProvider();
            var result = serviceProvider.GetRequiredService<IOptions<SimpleSettings>>().Value;

            Assert.NotNull(result);
            Assert.Equal(1075, result.IntegerA);
        }
        
        [Fact]
        public void Wires_up_SimpleAttributeValidatedSettings_from_Test1_file()
        {
            var services = new ServiceCollection();

            var configuration = ConfigurationTestBuilder.BuildFromEmbeddedResource(JsonIndex.AddSettingsTest1);
            
            services.AddSettings<SimpleSettings>(configuration);

            var serviceProvider = services.BuildServiceProvider();
            var result = serviceProvider.GetRequiredService<IOptions<SimpleSettings>>().Value;

            Assert.NotNull(result);
            Assert.Equal(89458, result.IntegerA);
        }
    }
}