using FluentMigrator.Runner.Initialization;
using Microsoft.Extensions.Options;
using OptionsPatternMvc.Dapper.Settings;

namespace OptionsPatternMvc.Dapper.Infrastructure
{
    /// <summary>Acts as the glue between getting the connection string via
    /// IOptions and FluentMigrator's configuration code.</summary>
    public class OpvDapperConnectionStringReader : IConnectionStringReader
    {
        private readonly IOptions<ConnectionStringsSettings> _connectionStringAccessor;

        public OpvDapperConnectionStringReader(
            IOptions<ConnectionStringsSettings> connectionStringAccessor
            )
        {
            _connectionStringAccessor = connectionStringAccessor;
        }
        
        public string GetConnectionString(string connectionStringOrName)
        {
            return _connectionStringAccessor.Value.OpvDapper;
        }

        /// <summary>Use a priority of at least 100 as there is code in FM.Core that sets up a
        /// default IConnectionStringReader with a priority of 100.</summary>
        public int Priority => 1000;
    }
}