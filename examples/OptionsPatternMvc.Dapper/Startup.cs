using System;
using System.Data;
using System.Data.SQLite;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OptionsPatternMvc.Dapper.Extensions;
using OptionsPatternMvc.Dapper.Settings;
using OptionsPatternValidation;

namespace OptionsPatternMvc.Dapper
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddValidatedSettings<ConnectionStringsSettings>(_configuration);
            services.AddValidatedSettings<DatabaseSettings>(_configuration);

            services.AddOpvDapperFluentMigrator();

            services.AddSingleton<Random>();

            services.AddScoped<IDbConnection>(provider =>
            {
                var connectionString =
                    provider.GetRequiredService<IOptions<ConnectionStringsSettings>>().Value.OpvDapper;
                return new SQLiteConnection(connectionString);
            });
            
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app, 
            IWebHostEnvironment env,
            IServiceProvider serviceProvider
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
