﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OptionsPatternMvc.Example.Database;
using OptionsPatternMvc.Example.Models;
using OptionsPatternMvc.Example.Representations;
using OptionsPatternMvc.Example.Settings;

namespace OptionsPatternMvc.Example.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly WeatherForecastContext _weatherForecastContext;
        private readonly ExampleAppSettings _exampleAppSettings;
        private readonly Random _random = new Random();

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger,
            IOptions<ExampleAppSettings> exampleAppSettingsAccessor,
            WeatherForecastContext weatherForecastContext
            )
        {
            _logger = logger;
            _weatherForecastContext = weatherForecastContext;
            _exampleAppSettings = exampleAppSettingsAccessor.Value;
        }

        [HttpGet]
        public IEnumerable<WeatherForecastRepresentation> Get()
        {
            Debug.WriteLine($"Settings.Name={_exampleAppSettings.Name}");
            
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecastRepresentation
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private WeatherForecast CreateForecast(int index)
        {
            return new WeatherForecast
            {
                ForecastTime = DateTimeOffset.UtcNow,
                Date = DateTime.Now.AddDays(index).Date,
                TemperatureC = _random.Next(-20, 55),
                Summary = Summaries[_random.Next(Summaries.Length)]
            };
        }
    }
}
