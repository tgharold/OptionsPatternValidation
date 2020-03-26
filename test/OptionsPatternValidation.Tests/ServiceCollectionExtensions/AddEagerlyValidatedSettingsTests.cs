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
    public class AddEagerlyValidatedSettingsTests
    {
        /// <summary>Demonstrate that AddValidatedSettings can work with POCOs that have no 
        /// DataAnnotation validation attributes on any properties.</summary>
        [Theory]
        [InlineData(JsonIndex.Unvalidated.Test1, 89458)]
        [InlineData(JsonIndex.Unvalidated.Test2, 892)]
        public void Wires_up_SimpleSettings_from_json_files(string filename, int integerA)
        {
            var services = new ServiceCollection();
            var configuration = ConfigurationTestBuilder.BuildFromEmbeddedResource(filename);

            services.AddEagerlyValidatedSettings<SimpleSettings>(configuration, out _);

            var serviceProvider = services.BuildServiceProvider();
            var result = serviceProvider.GetRequiredService<IOptions<SimpleSettings>>().Value;

            Assert.NotNull(result);
            Assert.Equal(integerA, result.IntegerA);
        }

        /// <summary>Check whether the initialResult object is the same as the
        /// one retrieved from the ServiceProvider later..</summary>
        [Theory]
        [InlineData(JsonIndex.Unvalidated.Test1, 89458)]
        [InlineData(JsonIndex.Unvalidated.Test2, 892)]
        public void Wires_up_SimpleSettings_from_json_files_ReferenceEquals(string filename, int integerA)
        {
            var services = new ServiceCollection();
            var configuration = ConfigurationTestBuilder.BuildFromEmbeddedResource(filename);

            services.AddEagerlyValidatedSettings<SimpleSettings>(configuration, out var initialResult);

            var serviceProvider = services.BuildServiceProvider();
            var result = serviceProvider.GetRequiredService<IOptions<SimpleSettings>>().Value;

            Assert.Equal(integerA, initialResult.IntegerA);
            Assert.Equal(integerA, result.IntegerA);
            
            Assert.True(object.ReferenceEquals(initialResult, result));
        } 
        
        [Fact]
        public void Catches_validation_error_for_Test1_file()
        {
            var services = new ServiceCollection();
            var configuration = ConfigurationTestBuilder
                .BuildFromEmbeddedResource(JsonIndex.AttributeValidated.Test1);
            var sectionName = SettingsSectionNameAttribute
                .GetSettingsSectionName<AttributeValidatedSettings>();
            
            // the next call will throw an exception 
            Action act = () =>
            {
                services.AddEagerlyValidatedSettings<AttributeValidatedSettings>(configuration, out _);
            };
            
            var exception = Assert.Throws<OptionsValidationException>(act);
            Assert.StartsWith("Validation failed for members:", exception.Message);
            
            /* Full validation message looks like
             *
             * Validation failed for members: 'IntegerA' with the error: 'The field IntegerA must be between 1
             * and 100.'.; Validation failed for members: 'BooleanB' with the error: 'The BooleanB field is
             * required.'.
             */
        }

        [Fact]
        public void Wires_up_settings_from_Test2_file()
        {
            var services = new ServiceCollection();

            var configuration = ConfigurationTestBuilder
                .BuildFromEmbeddedResource(JsonIndex.AttributeValidated.Test2);
            
            services.AddEagerlyValidatedSettings<AttributeValidatedSettings>(configuration, out _);

            var serviceProvider = services.BuildServiceProvider();
            var result = serviceProvider.GetRequiredService<IOptions<AttributeValidatedSettings>>().Value;

            Assert.NotNull(result);
            Assert.Equal(92, result.IntegerA);
            Assert.False(result.BooleanB);
        }
        
        [Fact]
        public void Wires_up_settings_from_Test2_file_ReferenceEquals()
        {
            var services = new ServiceCollection();

            var configuration = ConfigurationTestBuilder
                .BuildFromEmbeddedResource(JsonIndex.AttributeValidated.Test2);
            
            services.AddEagerlyValidatedSettings<AttributeValidatedSettings>(configuration, out var initialResult);

            var serviceProvider = services.BuildServiceProvider();
            var result = serviceProvider.GetRequiredService<IOptions<AttributeValidatedSettings>>().Value;

            Assert.Equal(92, initialResult.IntegerA);
            Assert.Equal(92, result.IntegerA);

            Assert.True(object.ReferenceEquals(initialResult, result));
        }
    }
}