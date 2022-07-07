using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OptionsPatternValidation.Tests.Helpers;
using OptionsPatternValidation.Tests.Json;
using OptionsPatternValidation.Tests.Settings.AttributeValidation;
using OptionsPatternValidation.Tests.Settings.Unvalidated;
using Xunit;

namespace OptionsPatternValidation.Tests.ServiceCollectionExtensions
{
    /// <summary>Test the DataAnnotations AddValidatedSettings(config) method.</summary>
    public class AddValidatedSettingsTests
    {
        [Fact]
        public void Wires_up_settings_from_string_as_IOptions()
        {
            var services = new ServiceCollection();
            
            const string json = @"
{
""AttributeValidated"": {
    ""IntegerA"": 76,
    ""BooleanB"": true
  }
}
                ";

            var configuration = ConfigurationTestBuilder.BuildFromJsonString(json);
            
            services.AddValidatedSettings<AttributeValidatedSettings>(configuration);

            var serviceProvider = services.BuildServiceProvider();
            var result = serviceProvider.GetRequiredService<IOptions<AttributeValidatedSettings>>().Value;

            Assert.NotNull(result);
            Assert.Equal(76, result.IntegerA);
            Assert.True(result.BooleanB);
        }

        [Fact]
        public void Wires_up_settings_from_string_as_IOptionsSnapshot()
        {
            var services = new ServiceCollection();
            
            const string json = @"
{
""AttributeValidated"": {
    ""IntegerA"": 23,
    ""BooleanB"": false
  }
}
                ";

            var configuration = ConfigurationTestBuilder.BuildFromJsonString(json);
            
            services.AddValidatedSettings<AttributeValidatedSettings>(configuration);

            var serviceProvider = services.BuildServiceProvider();
            var result = serviceProvider.GetRequiredService<IOptionsSnapshot<AttributeValidatedSettings>>().Value;

            Assert.NotNull(result);
            Assert.Equal(23, result.IntegerA);
            Assert.False(result.BooleanB);
        }

        [Fact]
        public void Wires_up_settings_from_string_as_IOptionsMonitor()
        {
            var services = new ServiceCollection();
            
            const string json = @"
{
""AttributeValidated"": {
    ""IntegerA"": 88,
    ""BooleanB"": false
  }
}
                ";

            var configuration = ConfigurationTestBuilder.BuildFromJsonString(json);
            
            services.AddValidatedSettings<AttributeValidatedSettings>(configuration);

            var serviceProvider = services.BuildServiceProvider();
            var result = serviceProvider.GetRequiredService<IOptionsMonitor<AttributeValidatedSettings>>().CurrentValue;

            Assert.NotNull(result);
            Assert.Equal(88, result.IntegerA);
            Assert.False(result.BooleanB);
        }

        /// <summary>Demonstrate that AddValidatedSettings can work with POCOs that have no 
        /// DataAnnotation validation attributes on any properties.</summary>
        [Theory]
        [InlineData(JsonIndex.Unvalidated.Test1, 89458)]
        [InlineData(JsonIndex.Unvalidated.Test2, 892)]
        public void Wires_up_SimpleSettings_from_json_files(string filename, int integerA)
        {
            var services = new ServiceCollection();
            var configuration = ConfigurationTestBuilder.BuildFromEmbeddedResource(filename);

            services.AddValidatedSettings<SimpleSettings>(configuration);

            var serviceProvider = services.BuildServiceProvider();
            var result = serviceProvider.GetRequiredService<IOptions<SimpleSettings>>().Value;

            Assert.NotNull(result);
            Assert.Equal(integerA, result.IntegerA);
        }
        
        [Fact]
        public void Catches_validation_error_for_Test1_file_using_IOptions()
        {
            var services = new ServiceCollection();
            
            var configuration = ConfigurationTestBuilder.BuildFromEmbeddedResource(
                JsonIndex.AttributeValidated.Test1);
            
            services.AddValidatedSettings<AttributeValidatedSettings>(configuration);
            
            var serviceProvider = services.BuildServiceProvider();

            // this call will work, because we're just getting the accessor singleton
            var optionsAccessor = serviceProvider.GetRequiredService<IOptions<AttributeValidatedSettings>>();
            // the next call will throw an exception because we're accessing the .Value instance
            Action act = () =>
            {
                var _ = optionsAccessor.Value;
            };
            
            var exception = Assert.Throws<OptionsValidationException>(act);
            Assert.StartsWith("Validation failed for members: 'IntegerA'", exception.Message);
        }

        [Fact]
        public void Catches_validation_error_for_Test1_file_using_IOptionsSnapshot()
        {
            var services = new ServiceCollection();
            
            var configuration = ConfigurationTestBuilder.BuildFromEmbeddedResource(
                JsonIndex.AttributeValidated.Test1);
            
            services.AddValidatedSettings<AttributeValidatedSettings>(configuration);
            
            var serviceProvider = services.BuildServiceProvider();

            // this call will work, because we're just getting the accessor singleton
            var optionsAccessor = serviceProvider.GetRequiredService<IOptionsSnapshot<AttributeValidatedSettings>>();
            // the next call will throw an exception because we're accessing the .Value instance
            Action act = () =>
            {
                var _ = optionsAccessor.Value;
            };
            
            var exception = Assert.Throws<OptionsValidationException>(act);
            Assert.StartsWith("Validation failed for members: 'IntegerA'", exception.Message);
        }

        [Fact]
        public void Catches_validation_error_for_Test1_file_using_IOptionsMonitor()
        {
            var services = new ServiceCollection();
            
            var configuration = ConfigurationTestBuilder.BuildFromEmbeddedResource(
                JsonIndex.AttributeValidated.Test1);
            
            services.AddValidatedSettings<AttributeValidatedSettings>(configuration);
            
            var serviceProvider = services.BuildServiceProvider();

            // this call will work, because we're just getting the accessor singleton
            var optionsAccessor = serviceProvider.GetRequiredService<IOptionsMonitor<AttributeValidatedSettings>>();
            // the next call will throw an exception because we're accessing the .CurrentValue instance
            Action act = () =>
            {
                var _ = optionsAccessor.CurrentValue;
            };
            
            var exception = Assert.Throws<OptionsValidationException>(act);
            Assert.StartsWith("Validation failed for members: 'IntegerA'", exception.Message);
        }

        [Fact]
        public void Wires_up_settings_from_Test2_file()
        {
            var services = new ServiceCollection();

            var configuration = ConfigurationTestBuilder.BuildFromEmbeddedResource(
                JsonIndex.AttributeValidated.Test2);
            
            services.AddValidatedSettings<AttributeValidatedSettings>(configuration);

            var serviceProvider = services.BuildServiceProvider();
            var result = serviceProvider.GetRequiredService<IOptions<AttributeValidatedSettings>>().Value;

            Assert.NotNull(result);
            Assert.Equal(92, result.IntegerA);
            Assert.False(result.BooleanB);
        }
    }
}