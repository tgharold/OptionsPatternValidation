using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OptionsPatternValidation.Tests.TestHelpers;
using OptionsPatternValidation.Tests.TestSettings.Unvalidated;
using OptionsPatternValidation.Tests.TestSettingsJson;
using Xunit;

namespace OptionsPatternValidation.Tests.ServiceCollectionExtensions
{
    /// <summary>Test the generic AddSettings(config) method.</summary>
    public class AddSettingsTests
    {
        [Fact]
        public void Wires_up_SimpleSettings_from_string()
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
        
        [Theory]
        [InlineData(JsonIndex.Unvalidated.Test1, 89458)]
        [InlineData(JsonIndex.Unvalidated.Test2, 892)]
        public void Wires_up_SimpleSettings_from_json_files(string filename, int integerA)
        {
            var services = new ServiceCollection();
            var configuration = ConfigurationTestBuilder.BuildFromEmbeddedResource(filename);

            services.AddSettings<SimpleSettings>(configuration);

            var serviceProvider = services.BuildServiceProvider();
            var result = serviceProvider.GetRequiredService<IOptions<SimpleSettings>>().Value;

            Assert.NotNull(result);
            Assert.Equal(integerA, result.IntegerA);
        }
    }
}