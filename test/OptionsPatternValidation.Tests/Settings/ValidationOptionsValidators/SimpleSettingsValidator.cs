using Microsoft.Extensions.Options;
using OptionsPatternValidation.Tests.Settings.Unvalidated;

namespace OptionsPatternValidation.Tests.Settings.ValidationOptionsValidators
{
    public class SimpleSettingsValidator : IValidateOptions<SimpleSettings>
    {
        public ValidateOptionsResult Validate(string name, SimpleSettings options)
        {
            throw new System.NotImplementedException();
        }
    }
}