using System;
using System.Data.Entity;
using OptionsPatternMvc.Example.Models;

namespace OptionsPatternMvc.Example.Database
{
    public class WeatherForecastContext : DbContext
    {
        public WeatherForecastContext(string connectionString)  : base(connectionString) { }

        public DbSet<WeatherForecast> WeatherForecasts { get; set; }
    }
}