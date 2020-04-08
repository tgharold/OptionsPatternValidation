using System;
using FluentMigrator;
using Microsoft.Extensions.Options;
using OptionsPatternMvc.Examples.EF6.Settings;

namespace OptionsPatternMvc.Examples.EF6.Migrations.Base
{
    public abstract class OpvExampleMigration : ForwardOnlyMigration
    {
        private readonly DatabaseSettings _databaseSettings;
        protected string SchemaName => _databaseSettings.SchemaNames.OpvExample1;

        protected OpvExampleMigration(IOptions<DatabaseSettings> databaseSettingsAccessor)
        {
            _databaseSettings = databaseSettingsAccessor.Value;
            
            if (string.IsNullOrEmpty(SchemaName))
                throw new Exception("Missing schema name.");
        }
    }
}