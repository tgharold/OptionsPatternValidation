using System;
using System.Data.Entity.Infrastructure;
using System.Data.SQLite;
using Microsoft.Extensions.Options;
using OptionsPatternMvc.Examples.EF6.Settings;

namespace OptionsPatternMvc.Examples.EF6.Database
{
    public class WeatherForecastContextFactory : IDbContextFactory<WeatherForecastContext>
    {
        private readonly IOptionsMonitor<ConnectionStringsSettings> _connectionStringsAccessor;

        public WeatherForecastContextFactory(IOptionsMonitor<ConnectionStringsSettings> connectionStringsAccessor)
        {
            _connectionStringsAccessor = connectionStringsAccessor;
        }

        public WeatherForecastContext Create()
        {
            var connectionString = _connectionStringsAccessor.CurrentValue.OpvExample1;
            
            if (string.IsNullOrEmpty(connectionString)) 
                throw new Exception("Missing connection string.");
            
            var connection = new SQLiteConnection(connectionString);
            
            return new WeatherForecastContext(connection);
        }
    }
}