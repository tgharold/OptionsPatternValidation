using System.ComponentModel.DataAnnotations;
using OptionsPatternValidation;

namespace OptionsPatternMvc.Examples.EF6.Settings
{
    [SettingsSectionName("ConnectionStrings")]
    public class ConnectionStringsSettings
    {
        [Required]
        public string OpvExample1 { get; set; }
    }
}