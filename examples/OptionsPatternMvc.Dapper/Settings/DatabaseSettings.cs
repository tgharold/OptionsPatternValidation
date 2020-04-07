using System.ComponentModel.DataAnnotations;
using OptionsPatternValidation;

namespace OptionsPatternMvc.Dapper.Settings
{
    [SettingsSectionName("Database")]
    public class DatabaseSettings
    {
        [Required]
        public string Schema { get; set; }
    }
}