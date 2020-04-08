using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.Options;
using OptionsPatternMvc.Example.Settings;

namespace OptionsPatternMvc.Example.Database
{
    public class OpvExample1VersionTableMetaData : DefaultVersionTableMetaData
    {
        private readonly IOptionsMonitor<DatabaseSettings> _databaseSettingsAccessor;

        public OpvExample1VersionTableMetaData(
            IOptionsMonitor<DatabaseSettings> databaseSettingsAccessor
            )
        {
            _databaseSettingsAccessor = databaseSettingsAccessor;
        }

        public override string SchemaName => _databaseSettingsAccessor.CurrentValue.SchemaNames.OpvExample1;
    }
}