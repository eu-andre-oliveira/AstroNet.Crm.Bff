using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Crosscutting.Data.Dapper
{
    [ExcludeFromCodeCoverage]
    public class DapperBase : IDapperBase
    {
        private readonly IConfiguration _configuration;

        public DapperBase(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static int TimeOut { get { return 2000; } }

        public void ExecuteProcedure(string databaseKey, string name, DatabaseClient databaseClient = DatabaseClient.SQLServer)
        {
            IDbConnection connection = ConnectionFactory.Connection(databaseKey, _configuration[$"ConnectionStrings:{databaseKey}"], databaseClient);
            connection.Open();
            connection.Execute(name, commandType: CommandType.StoredProcedure, commandTimeout: TimeOut);
            connection.Close();
        }

        public int ExecuteProcedure(string databaseKey, string name, object parameters, DatabaseClient databaseClient = DatabaseClient.SQLServer)
        {
            IDbConnection connection = ConnectionFactory.Connection(databaseKey, _configuration[$"ConnectionStrings:{databaseKey}"], databaseClient);
            connection.Open();
            int affectedRows = connection.Execute(name, parameters, commandType: CommandType.StoredProcedure, commandTimeout: TimeOut);
            connection.Close();
            return affectedRows;
        }

        public IEnumerable<T> ExecuteProcedure<T>(string databaseKey, string name, DatabaseClient databaseClient = DatabaseClient.SQLServer)
        {
            IDbConnection connection = ConnectionFactory.Connection(databaseKey, _configuration[$"ConnectionStrings:{databaseKey}"], databaseClient);
            connection.Open();
            var retorno = connection.Query<T>(name, commandType: CommandType.StoredProcedure, commandTimeout: TimeOut);
            connection.Close();
            return retorno;
        }

        public IEnumerable<T> ExecuteProcedure<T>(string databaseKey, string name, object parameters, DatabaseClient databaseClient = DatabaseClient.SQLServer)
        {
            IDbConnection connection = ConnectionFactory.Connection(databaseKey, _configuration[$"ConnectionStrings:{databaseKey}"], databaseClient);
            connection.Open();
            var retorno = connection.Query<T>(name, parameters, commandType: CommandType.StoredProcedure, commandTimeout: TimeOut);
            connection.Close();
            return (List<T>)retorno;
        }

        public IEnumerable<T> ExecuteProcedure<T>(string databaseKey, string name, Type[] types, Func<object[], T> map, object parameters, string splitOn = "Id", DatabaseClient databaseClient = DatabaseClient.SQLServer)
        {
            IDbConnection connection = ConnectionFactory.Connection(databaseKey, _configuration[$"ConnectionStrings:{databaseKey}"], databaseClient);
            connection.Open();
            var retorno = connection.Query<T>(name, types, map, parameters, splitOn: splitOn, commandType: CommandType.StoredProcedure, commandTimeout: TimeOut);
            connection.Close();
            return (List<T>)retorno;
        }

        public T ExecuteProcedureScalar<T>(string databaseKey, string name, object parameters, DatabaseClient databaseClient = DatabaseClient.SQLServer)
        {
            IDbConnection connection = ConnectionFactory.Connection(databaseKey, _configuration[$"ConnectionStrings:{databaseKey}"], databaseClient);
            connection.Open();
            var retorno = connection.Query<T>(name, parameters, commandType: CommandType.StoredProcedure, commandTimeout: TimeOut).FirstOrDefault();
            connection.Close();
            return retorno;
        }

        public T ExecuteProcedureParam<T>(string databaseKey, string name, DynamicParameters parameters, DatabaseClient databaseClient = DatabaseClient.SQLServer)
        {
            IDbConnection connection = ConnectionFactory.Connection(databaseKey, _configuration[$"ConnectionStrings:{databaseKey}"], databaseClient);
            connection.Open();
            var retorno = connection.Query<T>(name, parameters, commandType: CommandType.StoredProcedure, commandTimeout: TimeOut).FirstOrDefault();
            connection.Close();
            return retorno;
        }

        public async Task<IEnumerable<T>> ExecuteProcedureAsync<T>(string databaseKey, string name, object parameters, DatabaseClient databaseClient = DatabaseClient.SQLServer)
        {
            IDbConnection connection = ConnectionFactory.Connection(databaseKey, _configuration[$"ConnectionStrings:{databaseKey}"], databaseClient);
            connection.Open();
            var retorno = (await connection.QueryAsync<T>(name, parameters, commandType: CommandType.StoredProcedure, commandTimeout: TimeOut).ConfigureAwait(true)).ToList();
            connection.Close();
            return retorno;
        }

        public IEnumerable<T> GetAll<T>(string databaseKey, string tableName, DatabaseClient databaseClient = DatabaseClient.SQLServer)
        {
            IDbConnection connection = ConnectionFactory.Connection(databaseKey, _configuration[$"ConnectionStrings:{databaseKey}"], databaseClient);
            connection.Open();
            var retorno = connection.Query<T>(string.Format("select * from {0} where ativo = 1", tableName), commandTimeout: TimeOut);
            connection.Close();
            return retorno;
        }

        public IEnumerable<T> Get<T>(string databaseKey, string tableName, object parameters, DatabaseClient databaseClient = DatabaseClient.SQLServer)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("select * from {0}", tableName);
            var fisrtItem = parameters.GetType().GetProperties().FirstOrDefault();
            foreach (PropertyInfo item in parameters.GetType().GetProperties())
            {
                var value = AdjustValue((item.GetValue(parameters, null).GetType()).Name == "String" ? "'" + item.GetValue(parameters, null) + "'" : item.GetValue(parameters, null));
                sb.AppendFormat(" {0} {1} = {2}", (item == fisrtItem ? "WHERE" : "AND"), item.Name, value);

            }

            IDbConnection connection = ConnectionFactory.Connection(databaseKey, _configuration[$"ConnectionStrings:{databaseKey}"], databaseClient);
            connection.Open();
            var retorno = connection.Query<T>(sb.ToString(), commandTimeout: TimeOut);
            connection.Close();

            return retorno;
        }

        public IEnumerable<T> Get<T>(string databaseKey, object parameters, DatabaseClient databaseClient = DatabaseClient.SQLServer)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("select * from {0}", typeof(T).Name);
            var fisrtItem = parameters.GetType().GetProperties().FirstOrDefault();
            foreach (PropertyInfo item in parameters.GetType().GetProperties())
            {
                var value = AdjustValue((item.GetValue(parameters, null).GetType()).Name == "String" ? "'" + item.GetValue(parameters, null) + "'" : item.GetValue(parameters, null));
                sb.AppendFormat(" {0} {1} = {2}", (item == fisrtItem ? "WHERE" : "AND"), item.Name, value);
            }

            IDbConnection connection = ConnectionFactory.Connection(databaseKey, _configuration[$"ConnectionStrings:{databaseKey}"], databaseClient);
            connection.Open();
            var retorno = connection.Query<T>(sb.ToString(), commandTimeout: TimeOut);
            connection.Close();
            return retorno;
        }

        public IEnumerable<T> ExecuteSelect<T>(string databaseKey, string Command, DatabaseClient databaseClient = DatabaseClient.SQLServer)
        {
            IDbConnection connection = ConnectionFactory.Connection(databaseKey, _configuration[$"ConnectionStrings:{databaseKey}"], databaseClient);
            connection.Open();
            var retorno = connection.Query<T>(Command, commandType: CommandType.Text, commandTimeout: TimeOut, buffered: false);
            connection.Close();
            return retorno.ToList();
        }

        public IEnumerable<T> Query<T>(string databaseKey, string query, DatabaseClient databaseClient = DatabaseClient.SQLServer)
        {
            IDbConnection connection = ConnectionFactory.Connection(databaseKey, _configuration[$"ConnectionStrings:{databaseKey}"], databaseClient);
            connection.Open();
            var retorno = connection.Query<T>(query, commandTimeout: TimeOut);
            connection.Close();
            return retorno;
        }

        private object AdjustValue(object valor)
        {
            if (valor.GetType().Name == "Boolean")
            {
                if (valor.ToString() == "False")
                    return "0";
                else if (valor.ToString() == "True")
                    return "1";
            }

            return valor;
        }
    }
}