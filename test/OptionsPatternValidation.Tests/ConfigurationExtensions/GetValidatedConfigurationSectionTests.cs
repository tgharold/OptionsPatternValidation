using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OptionsPatternValidation.Tests.Helpers;
using OptionsPatternValidation.Tests.Settings.AttributeValidation;
using Xunit;

namespace OptionsPatternValidation.Tests.ConfigurationExtensions
{
    public class GetValidatedConfigurationSectionTests
    {
        [Fact]
        public void Wires_up_settings_from_string()
        {
            var services = new ServiceCollection();
            
            const string json = @"
{
""AttributeValidated"": {
    ""IntegerA"": 53,
    ""BooleanB"": false
  }
}
                ";

            var configuration = ConfigurationTestBuilder.BuildFromJsonString(json);

            var result = configuration.GetValidatedConfigurationSection<AttributeValidatedSettings>();
            
            services.AddValidatedSettings<AttributeValidatedSettings>(configuration);

            Assert.NotNull(result);
            Assert.Equal(53, result.IntegerA);
            Assert.False(result.BooleanB);
        }
        
        [Fact]
        public void Catches_validation_error_from_string()
        {
            var services = new ServiceCollection();
            
            const string json = @"
{
""AttributeValidated"": {
    ""IntegerA"": 153,
    ""BooleanB"": false
  }
}
                ";            
            
            var configuration = ConfigurationTestBuilder.BuildFromJsonString(json);

            void Act()
            {
                var _ = configuration.GetValidatedConfigurationSection<AttributeValidatedSettings>();
            }

            var exception = Assert.Throws<OptionsValidationException>(Act);
            Assert.StartsWith("Validation failed for members: 'IntegerA'", exception.Message);
        }
    }
}