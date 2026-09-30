using System;
using OptionsPatternValidation.Tests.Settings.AttributeValidation;
using OptionsPatternValidation.ValidationOptions;
using Xunit;

namespace OptionsPatternValidation.Tests.ValidationOptions
{
    public class RecursiveDataAnnotationValidateOptionsTests
    {
        [Fact]
        public void Throws_ArgumentNullException_for_null_options()
        {
            var validator = new RecursiveDataAnnotationValidateOptions<AttributeValidatedSettings>(null);

            var exception = Assert.Throws<ArgumentNullException>(() => validator.Validate(null, null));

            Assert.Equal("options", exception.ParamName);
        }

        [Fact]
        public void Skips_null_options_when_name_does_not_match()
        {
            var validator = new RecursiveDataAnnotationValidateOptions<AttributeValidatedSettings>("expected");

            var result = validator.Validate("other", null);

            Assert.True(result.Skipped);
        }
    }
}
