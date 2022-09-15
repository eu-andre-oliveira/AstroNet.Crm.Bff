using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infra.Crosscutting.Data.Dapper
{
    public interface IDapperBase
    {

        void ExecuteProcedure(string databaseKey, string name, DatabaseClient databaseClient = DatabaseClient.SQLServer);
        int ExecuteProcedure(string databaseKey, string name, object parameters, DatabaseClient databaseClient = DatabaseClient.SQLServer);
        IEnumerable<T> ExecuteProcedure<T>(string databaseKey, string name, DatabaseClient databaseClient = DatabaseClient.SQLServer);
        IEnumerable<T> ExecuteProcedure<T>(string databaseKey, string name, object parameters, DatabaseClient databaseClient = DatabaseClient.SQLServer);
        IEnumerable<T> ExecuteProcedure<T>(string databaseKey, string name, Type[] types, Func<object[], T> map, object parameters, string splitOn = "Id", DatabaseClient databaseClient = DatabaseClient.SQLServer);
        Task<IEnumerable<T>> ExecuteProcedureAsync<T>(string databaseKey, string name, object parameters, DatabaseClient databaseClient = DatabaseClient.SQLServer);
        IEnumerable<T> GetAll<T>(string databaseKey, string tableName, DatabaseClient databaseClient = DatabaseClient.SQLServer);
        IEnumerable<T> Get<T>(string databaseKey, string tableName, object parameters, DatabaseClient databaseClient = DatabaseClient.SQLServer);
        IEnumerable<T> Get<T>(string databaseKey, object parameters, DatabaseClient databaseClient = DatabaseClient.SQLServer);
        IEnumerable<T> Query<T>(string databaseKey, string query, DatabaseClient databaseClient = DatabaseClient.SQLServer);
        T ExecuteProcedureScalar<T>(string databaseKey, string name, object parameters, DatabaseClient databaseClient = DatabaseClient.SQLServer);
        T ExecuteProcedureParam<T>(string databaseKey, string name, DynamicParameters parameters, DatabaseClient databaseClient = DatabaseClient.SQLServer);
    }
}