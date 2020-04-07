using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OptionsPatternMvc.Dapper.Models;
using OptionsPatternMvc.Dapper.Representations;

namespace OptionsPatternMvc.Dapper.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly Random _random;
        private readonly IDbConnection _connection;

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger,
            Random random,
            IDbConnection connection
            )
        {
            _logger = logger;
            _random = random;
            _connection = connection;
        }

        [HttpGet]
        public IEnumerable<WeatherForecastRepresentation> Get()
        {
            var forecasts = Enumerable.Range(1, 5)
                .Select(index =>
                    CreateForecastForDay(DateTime.Now.AddDays(index).Date)
                )
                .ToArray();
            
            foreach (var forecast in forecasts)
            {
                _connection.Insert(forecast);
            }

            return forecasts.Select(x => x.ToRepresentation());
        }
        
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
