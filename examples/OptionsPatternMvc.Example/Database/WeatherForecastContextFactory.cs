using System.Data.Entity.Infrastructure;
using Microsoft.Extensions.Options;
using OptionsPatternMvc.Example.Settings;

namespace OptionsPatternMvc.Example.Database
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
            return new WeatherForecastContext(connectionString);
        }
    }
}