using System.ComponentModel.DataAnnotations;
using OptionsPatternValidation;

namespace OptionsPatternMvc.Dapper.Settings
{
    [SettingsSectionName("ConnectionStrings")]
    public class ConnectionStringsSettings
    {
        [Required]
        public string OpvDapper { get; set; }
    }
}