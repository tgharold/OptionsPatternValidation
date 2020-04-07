using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.Options;
using OptionsPatternMvc.Dapper.Settings;

namespace OptionsPatternMvc.Dapper.Infrastructure
{
    public class FluentMigratorVersionTableMetaData : IVersionTableMetaData
    {
        private readonly IOptions<DatabaseSettings> _databaseSettingsAccessor;

        public FluentMigratorVersionTableMetaData(
            IOptions<DatabaseSettings> databaseSettingsAccessor
            )
        {
            _databaseSettingsAccessor = databaseSettingsAccessor;
        }
        
        public object ApplicationContext { get; set; }
        
        public bool OwnsSchema => true;

        public string SchemaName => _databaseSettingsAccessor.Value.Schema;

        public string TableName => "VersionInfo";
        
        public string ColumnName => "Version";
        
        public string DescriptionColumnName => "Description";
        
        public string UniqueIndexName => "UC_Version";
        
        public string AppliedOnColumnName => "AppliedOn";
    }
}