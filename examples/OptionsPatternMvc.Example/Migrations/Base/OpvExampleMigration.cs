using FluentMigrator;
using Microsoft.Extensions.Options;
using OptionsPatternMvc.Example.Settings;

namespace OptionsPatternMvc.Example.Migrations.Base
{
    public abstract class OpvExampleMigration : ForwardOnlyMigration
    {
        private readonly DatabaseSettings _databaseSettings;
        protected string SchemaName => _databaseSettings.SchemaNames.OpvExample1;

        protected OpvExampleMigration(IOptions<DatabaseSettings> databaseSettingsAccessor)
        {
            _databaseSettings = databaseSettingsAccessor.Value;
        }
    }
}