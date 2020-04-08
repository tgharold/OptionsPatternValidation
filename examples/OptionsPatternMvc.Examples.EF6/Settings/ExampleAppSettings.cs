using System.ComponentModel.DataAnnotations;
using OptionsPatternValidation;

namespace OptionsPatternMvc.Examples.EF6.Settings
{
    [SettingsSectionName("Example")]
    public class ExampleAppSettings
    {
        [Required]
        public string Name { get; set; }
    }
}