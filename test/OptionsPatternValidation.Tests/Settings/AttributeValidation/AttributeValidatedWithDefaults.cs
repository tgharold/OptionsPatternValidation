using System.ComponentModel.DataAnnotations;

namespace OptionsPatternValidation.Tests.Settings.AttributeValidation
{
    /// <summary>This POCO tests out DataAnnotation validation and every
    /// property should have some form of validation attribute. But
    /// the default values for the properties should pass validation.</summary>
    public class AttributeValidatedWithDefaults
    {
        [Required, Range(1, 100)]
        public int? IntegerA { get; set; } = 35;
        
        [Required]
        public bool? BooleanB { get; set; } = true;
            
        [Required]
        public string ConnectionString { get; set; } = "some-connection-string";
    }
}