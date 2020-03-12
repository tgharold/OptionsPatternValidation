using System;
using Microsoft.Extensions.Configuration;

namespace OptionsPatternValidation.Tests.TestHelpers
{
    public static class ConfigurationTestBuilder
    {
        public static IConfiguration BuildFromJsonString(string json)
        {
            IConfiguration configuration;
            
            if (json is null) throw new ArgumentNullException(nameof(json));
            
            using (var stream = json.ToStream())
            {
                var builder = new ConfigurationBuilder()
                    .AddJsonStream(stream);
                configuration = builder.Build();
            }

            return configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public static IConfiguration BuildFromFile(string fileName)
        {
            if (fileName is null) throw new ArgumentNullException(nameof(fileName));
            
            var builder = new ConfigurationBuilder()
                .AddJsonFile(fileName);

            return builder.Build();
        }
    }
}