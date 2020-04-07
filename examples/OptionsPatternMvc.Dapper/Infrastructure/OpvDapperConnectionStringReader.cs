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

        /// <summary>Returns the connection string.</summary>
        /// <param name="_">In current versions of FluentMigrator, this will never be
        /// a connection string, but may be the name of a FM processor.  There is
        /// code in FM that looks for a prefix of "SqlServer" on the processor
        /// name to determine whether it is executing against Microsoft SQL Server.
        /// This value defaults to null if not specified during setup.</param>
        public string GetConnectionString(string _)
        {
            return _connectionStringAccessor.Value.OpvDapper;
        }

        /// <summary>Use a priority of at least 100 as there is code in FM.Core that sets up a
        /// default IConnectionStringReader with a priority of 100.  See the FluentMigrator
        /// ConnectionStringAccessor class for details on how priority is used.  Higher
        /// values get used first.</summary>
        public int Priority => 1000;
    }
}