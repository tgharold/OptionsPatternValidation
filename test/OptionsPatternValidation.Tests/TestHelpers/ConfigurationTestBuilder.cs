using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

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

        public static IConfiguration BuildFromEmbeddedResource(string fileName)
        {
            IConfiguration configuration;
            
            if (fileName is null) throw new ArgumentNullException(nameof(fileName));
            
            var embeddedProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
            using (var stream = embeddedProvider.GetFileInfo(fileName).CreateReadStream())
            {
                var builder = new ConfigurationBuilder()
                    .AddJsonStream(stream);
                configuration = builder.Build();
            }

            return configuration;
        }
    }
}