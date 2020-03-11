using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OptionsPatternValidation.Tests.AttributeValidatedSettings;
using Xunit;

namespace OptionsPatternValidation.Tests.ServiceCollectionExtensions
{
    public class AddSettingsTests
    {
        [Fact]
        public void Wires_up_SimpleAttributeValidatedSettings()
        {
            var services = new ServiceCollection();
            Stream stream = null; //TODO: Create a JSON string to feed into ConfigurationBuilder
            var builder = new ConfigurationBuilder()
                .AddJsonStream(stream);
            var configuration = builder.Build();
            
            services.AddSettings<SimpleAttributeValidatedSettings>(configuration);

            var serviceProvider = services.BuildServiceProvider();
            var result = serviceProvider.GetRequiredService<IOptions<SimpleAttributeValidatedSettings>>().Value;

            Assert.NotNull(result);
        }
        
    }
}