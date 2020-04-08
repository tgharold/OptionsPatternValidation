using System.Data.Entity;
using System.Data.SQLite;
using OptionsPatternMvc.Examples.EF6.Models;

namespace OptionsPatternMvc.Examples.EF6.Database
{
    [DbConfigurationType(typeof(SqliteDbConfiguration))]
    public class WeatherForecastContext : DbContext
    {
        public WeatherForecastContext(SQLiteConnection connection)  : base(connection, true) { }

        public DbSet<WeatherForecast> WeatherForecasts { get; set; }
    }
}