using System.ComponentModel.DataAnnotations;

namespace OptionsPatternMvc.Dapper.Settings
{
    public class ConnectionStringsSettings
    {
        [Required]
        public string OpvDapper { get; set; }
    }
}