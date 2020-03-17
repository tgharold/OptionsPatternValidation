using System.Data.Entity.Infrastructure;

namespace OptionsPatternMvc.Example.Database
{
    public class WeatherForecastContextFactory : IDbContextFactory< WeatherForecastContext>
    {
        public WeatherForecastContext Create()
        {
            return new WeatherForecastContext();
        }
    }
}