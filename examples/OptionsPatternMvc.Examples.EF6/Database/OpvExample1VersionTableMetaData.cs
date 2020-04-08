using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.Options;
using OptionsPatternMvc.Examples.EF6.Settings;

namespace OptionsPatternMvc.Examples.EF6.Database
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