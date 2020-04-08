using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.Options;
using OptionsPatternMvc.Examples.EF6.Settings;

namespace OptionsPatternMvc.Examples.EF6.Database
{
    public class OpvExample1VersionTableMetaData : IVersionTableMetaData
    {
        private readonly IOptions<DatabaseSettings> _databaseSettingsAccessor;

        public OpvExample1VersionTableMetaData(
            IOptions<DatabaseSettings> databaseSettingsAccessor
            )
        {
            _databaseSettingsAccessor = databaseSettingsAccessor;
        }

        public object ApplicationContext { get; set; }
        
        public bool OwnsSchema => true;

        public string SchemaName => _databaseSettingsAccessor.Value.SchemaNames.OpvExample1;

        public string TableName => "VersionInfo";
        
        public string ColumnName => "Version";
        
        public string DescriptionColumnName => "Description";
        
        public string UniqueIndexName => "UC_Version";
        
        public string AppliedOnColumnName => "AppliedOn";
    }
}