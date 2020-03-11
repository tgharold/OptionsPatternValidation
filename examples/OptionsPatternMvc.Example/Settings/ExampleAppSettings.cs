using System.ComponentModel.DataAnnotations;
using OptionsPatternValidation.Attributes;

namespace OptionsPatternMvc.Example.Settings
{
    [SettingsSectionName("Example")]
    public class ExampleAppSettings
    {
        [Required]
        public string Name { get; set; }
    }
}