using System.Data.Entity;
using OptionsPatternMvc.Example.Models;

namespace OptionsPatternMvc.Example.Database
{
    public class WeatherForecastContext : DbContext
    {
        public DbSet<WeatherForecast> WeatherForecasts { get; set; }
        
    }
}