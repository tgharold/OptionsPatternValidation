using System.ComponentModel.DataAnnotations;

namespace OptionsPatternValidation.Tests.TestSettings.AttributeValidation
{
    [SettingsSectionName("AttributeValidated")]
    public class AttributeValidatedSettings
    {
        [Required, Range(1, 100)]
        public int? IntegerA { get; set; }
        
        [Required]
        public bool? BooleanB { get; set; }
    }
}