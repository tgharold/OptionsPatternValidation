using System;

namespace OptionsPatternMvc.Examples.EF6.Representations
{
    public class WeatherForecastRepresentation
    {
        public DateTimeOffset ForecastTime { get; set; }

        public DateTimeOffset Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }        
    }
}