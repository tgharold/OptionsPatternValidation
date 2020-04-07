using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger,
            Random random
            )
        {
            _logger = logger;
            _random = random;
        }

        [HttpGet]
        public IEnumerable<WeatherForecastRepresentation> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecastRepresentation
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
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
