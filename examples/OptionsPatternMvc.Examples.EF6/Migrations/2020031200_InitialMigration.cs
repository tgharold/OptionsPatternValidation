using FluentMigrator;
using Microsoft.Extensions.Options;
using OptionsPatternMvc.Examples.EF6.Migrations.Base;
using OptionsPatternMvc.Examples.EF6.Settings;

namespace OptionsPatternMvc.Examples.EF6.Migrations
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
                .WithColumn("Id").AsInt32().Identity().PrimaryKey().NotNullable()
                .WithColumn("ForecastTime").AsDateTime().NotNullable()
                .WithColumn("Date").AsDate().NotNullable()
                .WithColumn("TemperatureC").AsInt32().NotNullable()
                .WithColumn("Summary").AsString(50).NotNullable()
                ;
        }
    }
}