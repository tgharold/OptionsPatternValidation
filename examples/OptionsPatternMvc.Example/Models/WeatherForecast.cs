using System;
using OptionsPatternMvc.Example.Representations;

namespace OptionsPatternMvc.Example.Models
{
    public class WeatherForecast
    {
        public int Id { get; set; }
        
        public DateTime ForecastTime { get; set; }
        
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public string Summary { get; set; }
        
        public WeatherForecastRepresentation ToRepresentation()
        {
            return new WeatherForecastRepresentation
            {
                ForecastTime = ForecastTime,
                Date = Date,
                TemperatureC = TemperatureC,
                Summary = Summary
            };
        }
    }
}
