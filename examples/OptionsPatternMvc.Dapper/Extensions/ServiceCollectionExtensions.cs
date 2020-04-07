using FluentMigrator.Runner;
using FluentMigrator.Runner.Conventions;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OptionsPatternMvc.Dapper.Infrastructure;
using OptionsPatternMvc.Dapper.Migrations;
using OptionsPatternMvc.Dapper.Settings;

namespace OptionsPatternMvc.Dapper.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Wire up FluentMigrator.</summary>
        public static IServiceCollection AddOpvDapperFluentMigrator(this IServiceCollection services)
        {
            services.AddSingleton<IAssemblySourceItem>(provider =>
                new AssemblySourceItem(
                    typeof(InitialMigration).Assembly
                ));

            services.AddSingleton<IConnectionStringReader>(provider =>
                new OpvDapperConnectionStringReader(
                    provider.GetRequiredService<IOptions<ConnectionStringsSettings>>()
                ));

            services.AddSingleton<IVersionTableMetaData>(provider =>
                new FluentMigratorVersionTableMetaData(
                    provider.GetRequiredService<IOptions<DatabaseSettings>>()
                ));

            services
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb.AddSQLite())
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                ;
            
            // This must be called after AddFluentMigratorCore() as it also registers a IConventionSet
            services.AddSingleton<IConventionSet>(provider =>
                new DefaultConventionSet(
                    provider.GetRequiredService<IOptions<DatabaseSettings>>().Value.Schema,
                    workingDirectory: null
                ));

            return services;
        }
    }
}