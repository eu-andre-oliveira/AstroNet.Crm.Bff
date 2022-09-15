using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Crosscutting.Data.Dapper
{
    [ExcludeFromCodeCoverage]
    public static class ConnectionFactory
    {
        public static IDbConnection Connection(string databaseKey, string connectionString, DatabaseClient databaseClient = DatabaseClient.SQLServer)
        {
            return new SqlConnection(connectionString);
        }
    }
}