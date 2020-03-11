using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OptionsPatternValidation.Tests.AttributeValidatedSettings;
using OptionsPatternValidation.Tests.TestHelpers;
using Xunit;

namespace OptionsPatternValidation.Tests.ServiceCollectionExtensions
{
    /// <summary>Test the generic AddSettings(config) method.</summary>
    public class AddSettingsTests
    {
        [Fact]
        public void Wires_up_SimpleAttributeValidatedSettings()
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
            
            services.AddSettings<SimpleAttributeValidatedSettings>(configuration);

            var serviceProvider = services.BuildServiceProvider();
            var result = serviceProvider.GetRequiredService<IOptions<SimpleAttributeValidatedSettings>>().Value;

            Assert.NotNull(result);
            Assert.Equal(1075, result.IntegerA);
        }
    }
}