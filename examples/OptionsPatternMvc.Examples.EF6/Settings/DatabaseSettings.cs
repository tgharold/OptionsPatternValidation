using System.ComponentModel.DataAnnotations;
using OptionsPatternValidation;

namespace OptionsPatternMvc.Examples.EF6.Settings
{
    [SettingsSectionName("Database")]
    public class DatabaseSettings
    {
        public DatabaseSchemaNames SchemaNames { get; set; } = new DatabaseSchemaNames();
        
        public class DatabaseSchemaNames
        {
            [Required]
            public string OpvExample1 { get; set; } = "opv1";
        }
    }
}