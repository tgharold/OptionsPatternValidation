using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OptionsPatternMvc.Examples.EF6.Representations;
using OptionsPatternMvc.Examples.EF6.Services;
using OptionsPatternMvc.Examples.EF6.Settings;

namespace OptionsPatternMvc.Examples.EF6.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly WeatherForecastService _weatherForecastService;
        private readonly ExampleAppSettings _exampleAppSettings;
        public WeatherForecastController(
            ILogger<WeatherForecastController> logger,
            IOptions<ExampleAppSettings> exampleAppSettingsAccessor,
            WeatherForecastService weatherForecastService
            )
        {
            _logger = logger;
            _weatherForecastService = weatherForecastService;
            _exampleAppSettings = exampleAppSettingsAccessor.Value;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecastRepresentation>> Get()
        {
            Debug.WriteLine($"Settings.Name={_exampleAppSettings.Name}");

            var forecastDates = Enumerable
                .Range(1, 5)
                .Select(index => DateTime.Now.AddDays(index).Date)
                .ToList();
            
            var forecasts = await _weatherForecastService.GetForecastAsync(forecastDates);

            return forecasts.Select(x => new WeatherForecastRepresentation
            {
               Date = x.Date,
               Summary = x.Summary,
               ForecastTime = x.ForecastTime,
               TemperatureC = x.TemperatureC
            });
        }
    }
}
