using System.ComponentModel.DataAnnotations;
using OptionsPatternValidation;

namespace OptionsPatternMvc.Example.Settings
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