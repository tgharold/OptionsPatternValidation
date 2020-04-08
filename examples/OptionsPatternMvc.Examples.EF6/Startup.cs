using System;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OptionsPatternMvc.Examples.EF6.Database;
using OptionsPatternMvc.Examples.EF6.Migrations;
using OptionsPatternMvc.Examples.EF6.Services;
using OptionsPatternMvc.Examples.EF6.Settings;
using OptionsPatternValidation;

namespace OptionsPatternMvc.Examples.EF6
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddValidatedSettings<ConnectionStringsSettings>(Configuration);
            services.AddValidatedSettings<DatabaseSettings>(Configuration);
            services.AddValidatedSettings<ExampleAppSettings>(Configuration);

            services.AddScoped<WeatherForecastService>();

            services.AddSingleton(provider =>
                {
                    var connectionStringsAccessor = provider.GetService<IOptionsMonitor<ConnectionStringsSettings>>();
                    return new WeatherForecastContextFactory(connectionStringsAccessor);
                });

            services.AddSingleton<IConnectionStringReader>(provider =>
                {
                    var connectionStringsAccessor = provider.GetService<IOptions<ConnectionStringsSettings>>();
                    return new OpvExample1ConnectionStringReader(connectionStringsAccessor);
                });
            
            services
                .AddFluentMigratorCore();

            services
                .ConfigureRunner(
                    builder => builder
                        .AddSQLite()
                        .ScanIn(typeof(InitialMigration).Assembly).For.Migrations())
                ;
            
            services
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                ;            
            
            services.AddControllers();
            
            // Poking around in the services collection via a debug can look like
            // services.Where(x => x.ServiceType.Name.Contains("Runner", StringComparison.OrdinalIgnoreCase))
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IServiceProvider serviceProvider,
            IWebHostEnvironment env
            )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            serviceProvider.GetRequiredService<IMigrationRunner>().MigrateUp();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
