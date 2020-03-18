using FluentMigrator;
using Microsoft.Extensions.Options;
using OptionsPatternMvc.Example.Migrations.Base;
using OptionsPatternMvc.Example.Settings;

namespace OptionsPatternMvc.Example.Migrations
{
    [Migration(2020031200)]
    public class InitialMigration : OpvExampleMigration
    {
        public InitialMigration(IOptions<DatabaseSettings> databaseSettingsAccessor)
            : base (databaseSettingsAccessor) { }
        
        public override void Up()
        {
            Create
                .Table("WeatherForecasts")
                .InSchema(SchemaName)
                .WithColumn("Id").AsInt32().PrimaryKey().NotNullable()
                .WithColumn("ForecastTime").AsDateTime().NotNullable()
                .WithColumn("Date").AsDate().NotNullable()
                .WithColumn("TemperatureC").AsInt32().NotNullable()
                .WithColumn("Summary").AsString(50).NotNullable()
                ;
        }
    }
}