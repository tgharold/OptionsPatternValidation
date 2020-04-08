using System.ComponentModel.DataAnnotations;
using OptionsPatternValidation;

namespace OptionsPatternMvc.Example.Settings
{
    [SettingsSectionName("ConnectionStrings")]
    public class ConnectionStringsSettings
    {
        [Required]
        public string OpvExample1 { get; set; }
    }
}