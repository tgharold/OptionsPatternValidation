using System.ComponentModel.DataAnnotations;

namespace OptionsPatternMvc.Dapper.Settings
{
    public class DatabaseSettings
    {
        [Required]
        public string Schema { get; set; }
    }
}