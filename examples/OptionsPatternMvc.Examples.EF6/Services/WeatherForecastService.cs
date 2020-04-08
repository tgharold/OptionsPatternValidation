using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using OptionsPatternMvc.Examples.EF6.Database;
using OptionsPatternMvc.Examples.EF6.Models;

namespace OptionsPatternMvc.Examples.EF6.Services
{
    public class WeatherForecastService
    {
        private readonly WeatherForecastContextFactory _weatherForecastContextFactory;
        private readonly Random _random = new Random();

        public WeatherForecastService(WeatherForecastContextFactory weatherForecastContextFactory)
        {
            _weatherForecastContextFactory = weatherForecastContextFactory;
        }

        public async Task<IEnumerable<WeatherForecast>> GetForecastAsync(List<DateTime> dates)
        {
            List<WeatherForecast> forecasts;
            
            using (var context = _weatherForecastContextFactory.Create())
            {
                forecasts = await context.WeatherForecasts
                    .Where(x => dates.Contains(x.Date))
                    .ToListAsync();

                var missingDates = dates
                    .Where(x => !forecasts.Select(y => y.Date).Contains(x.Date));

                var newForecasts = missingDates
                    .Select(CreateForecastForDay).ToList();
                forecasts.AddRange(newForecasts);
                context.WeatherForecasts.AddRange(newForecasts);
                await context.SaveChangesAsync();
            }

            return forecasts.OrderBy(x => x.Date).ToList();
        }
        
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        
        private WeatherForecast CreateForecastForDay(DateTime date)
        {
            return new WeatherForecast
            {
                ForecastTime = DateTime.UtcNow,
                Date = date,
                TemperatureC = _random.Next(-20, 55),
                Summary = Summaries[_random.Next(Summaries.Length)]
            };
        }
    }
}