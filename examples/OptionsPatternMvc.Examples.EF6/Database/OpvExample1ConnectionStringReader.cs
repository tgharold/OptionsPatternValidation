using FluentMigrator.Runner.Initialization;
using Microsoft.Extensions.Options;
using OptionsPatternMvc.Examples.EF6.Settings;

namespace OptionsPatternMvc.Examples.EF6.Database
{
    /// <summary>This provides the FluentMigrator runner with a way to get the connection string.</summary>
    public class OpvExample1ConnectionStringReader : IConnectionStringReader
    {
        private readonly ConnectionStringsSettings _connectionStringSettings;

        public OpvExample1ConnectionStringReader(
            IOptions<ConnectionStringsSettings> connectionSettingsAccessor
            )
        {
            _connectionStringSettings = connectionSettingsAccessor.Value;
        }
        
        public string GetConnectionString(string connectionStringOrName)
        {

            return _connectionStringSettings.OpvExample1;
        }

        public int Priority { get; } = 1000;
    }
}