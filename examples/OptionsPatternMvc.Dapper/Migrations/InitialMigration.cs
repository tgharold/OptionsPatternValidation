using FluentMigrator;

namespace OptionsPatternMvc.Dapper.Migrations
{
    [Migration(20200407001)]
    public class InitialMigration : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create
                .Table("WeatherForecasts")
                .WithColumn("Id").AsInt32().Identity().PrimaryKey().NotNullable()
                .WithColumn("ForecastTime").AsDateTime().NotNullable()
                .WithColumn("Date").AsDate().NotNullable()
                .WithColumn("TemperatureC").AsInt32().NotNullable()
                .WithColumn("Summary").AsString(50).NotNullable()
                ;
        }
    }
}