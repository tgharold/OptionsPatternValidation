using System.Data.Entity;
using System.Data.SQLite;
using OptionsPatternMvc.Example.Models;

namespace OptionsPatternMvc.Example.Database
{
    [DbConfigurationType(typeof(SqliteDbConfiguration))]
    public class WeatherForecastContext : DbContext
    {
        public WeatherForecastContext(SQLiteConnection connection)  : base(connection, true) { }

        public DbSet<WeatherForecast> WeatherForecasts { get; set; }
    }
}