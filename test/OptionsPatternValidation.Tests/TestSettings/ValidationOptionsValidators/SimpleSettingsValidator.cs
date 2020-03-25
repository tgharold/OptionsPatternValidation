using Microsoft.Extensions.Options;
using OptionsPatternValidation.Tests.TestSettings.Unvalidated;

namespace OptionsPatternValidation.Tests.TestSettings.ValidationOptionsValidators
{
    public class SimpleSettingsValidator : IValidateOptions<SimpleSettings>
    {
        public ValidateOptionsResult Validate(string name, SimpleSettings options)
        {
            throw new System.NotImplementedException();
        }
    }
}