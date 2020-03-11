using System.ComponentModel.DataAnnotations;
using OptionsPatternValidation;

namespace OptionsPatternMvc.Example.Settings
{
    [SettingsSectionName("Example")]
    public class ExampleAppSettings
    {
        [Required]
        public string Name { get; set; }
    }
}