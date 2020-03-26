using System.ComponentModel.DataAnnotations;

namespace OptionsPatternValidation.Tests.Settings.AttributeValidation
{
    /// <summary>This POCO tests out DataAnnotation validation and every
    /// property should have some form of validation attribute.</summary>
    [SettingsSectionName("AttributeValidated")]
    public class AttributeValidatedSettings
    {
        [Required, Range(1, 100)]
        public int? IntegerA { get; set; }
        
        [Required]
        public bool? BooleanB { get; set; }
    }
}