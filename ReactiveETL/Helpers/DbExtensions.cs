using System;
using System.Data;
using ReactiveETL.Activators;
using ReactiveETL.Operations.Database;

namespace ReactiveETL
{
    /// <summary>
    /// Extension methods for database operations
    /// </summary>
    public static class DbExtensions
    {
        /// <summary>
        /// Apply a command operation
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="activator">command parameters</param>
        /// <returns>command operation</returns>
        public static CommandOperation DbCommand(this IObservableOperation observed, CommandActivator activator)
        {
            CommandOperation cmd = new CommandOperation(activator);
            observed.Subscribe(cmd);
            return cmd;
        }

        /// <summary>
        /// Apply a command operation
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="act">callback on command activator</param>
        /// <returns>command operation</returns>
        public static CommandOperation DbCommand(this IObservableOperation observed, Action<CommandActivator> act)
        {
            var activator = new CommandActivator();
            act(activator);
            CommandOperation cmd = new CommandOperation(activator);
            observed.Subscribe(cmd);
            return cmd;
        }

        /// <summary>
        /// Apply a command operation
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="connStr">Name of a connection string defined in the application configuration file</param>
        /// <param name="CommandText">text of the command</param>
        /// <returns>command operation</returns>
        public static CommandOperation DbCommand(this IObservableOperation observed, string connStr, string CommandText)
        {
            return observed.DbCommand(connStr, CommandText, false, null);
        }

        /// <summary>
        /// Apply a command operation
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="connection">Database connection</param>
        /// <param name="CommandText">text of the command</param>
        /// <returns>command operation</returns>
        public static CommandOperation DbCommand(this IObservableOperation observed, IDbConnection connection, string CommandText)
        {
            return observed.DbCommand(connection, CommandText, false, null);
        }

        /// <summary>
        /// Apply a command operation
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="connStr">name of a connection string defined in the application configuration file</param>
        /// <param name="prepare">callback method to prepare the command</param>
        /// <returns>command operation</returns>
        public static CommandOperation DbCommand(this IObservableOperation observed, string connStr, Action<IDbCommand, Row> prepare)
        {
            return observed.DbCommand(connStr, null, false, prepare);
        }

        /// <summary>
        /// Apply a command operation
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="connection">Database connection</param>
        /// <param name="prepare">callback method to prepare the command</param>
        /// <returns>command operation</returns>
        public static CommandOperation DbCommand(this IObservableOperation observed, IDbConnection connection, Action<IDbCommand, Row> prepare)
        {
            return observed.DbCommand(connection, null, false, prepare);
        }

        /// <summary>
        /// Apply a command operation
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="connStr">Name of a connection string defined in the application configuration file</param>
        /// <param name="CommandText">text of the command</param>
        /// <param name="prepare">callback method to prepare the command</param>
        /// <returns>command operation</returns>
        public static CommandOperation DbCommand(this IObservableOperation observed, string connStr, string CommandText, Action<IDbCommand, Row> prepare)
        {
            return observed.DbCommand(connStr, null, false, prepare);
        }

        /// <summary>
        /// Apply a command operation
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="connection">Database connection</param>
        /// <param name="CommandText">text of the command</param>
        /// <param name="prepare">callback method to prepare the command</param>
        /// <returns>command operation</returns>
        public static CommandOperation DbCommand(this IObservableOperation observed, IDbConnection connection, string CommandText, Action<IDbCommand, Row> prepare)
        {
            return observed.DbCommand(connection, null, false, prepare);
        }

        /// <summary>
        /// Apply a command operation
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="connection">Database connection</param>
        /// <param name="CommandText">text of the command</param>
        /// <param name="isQuery">indicate if the command is a query</param>
        /// <param name="prepare">callback method to prepare the command</param>
        /// <returns>command operation</returns>
        public static CommandOperation DbCommand(this IObservableOperation observed, IDbConnection connection, string CommandText, bool isQuery, Action<IDbCommand, Row> prepare)
        {
            CommandActivator activator = new CommandActivator();

            activator.Connection = connection;
            activator.CommandText = CommandText;
            activator.Prepare = prepare;
            activator.IsQuery = isQuery;

            return observed.DbCommand(activator);
        }

        /// <summary>
        /// Apply a command operation
        /// </summary>
        /// <param name="observed">observed operation</param>
        /// <param name="connStr">Name of a connection string defined in the application configuration file</param>
        /// <param name="CommandText">text of the command</param>
        /// <param name="isQuery">indicate if the command is a query</param>
        /// <param name="prepare">callback method to prepare the command</param>
        /// <returns>command operation</returns>
        public static CommandOperation DbCommand(this IObservableOperation observed, string connStr, string CommandText, bool isQuery, Action<IDbCommand, Row> prepare)
        {
            CommandActivator activator = new CommandActivator();

            activator.ConnStringName = connStr;
            activator.CommandText = CommandText;
            activator.Prepare = prepare;
            activator.IsQuery = isQuery;

            return observed.DbCommand(activator);
        }

        /// <summary>
        /// upsert into mongodb。處理寫入mongodb。
        /// </summary>
        /// <typeparam name="T">mongodb collection 對應到C# class</typeparam>
        /// <param name="observed">observed operation</param>
        /// <param name="database">mongodb IMongoDatabase</param>
        /// <param name="collectionName">collection name</param>
        /// <param name="filter">呼叫方要upsert的過濾條件</param>
        /// <param name="update">每次provider push出來是Row類型，透過委派由呼叫方完成寫入update definition</param>
        /// <param name="options">mongodb update option</param>
        /// <returns></returns>
        public static MongoDbUpdateOperation<T> MongoDbUpsert<T>(this IObservableOperation observed,
            MongoDB.Driver.IMongoDatabase database,
            string collectionName,
            Func<Row, MongoDB.Driver.FilterDefinition<T>> filter,
            Func<Row, MongoDB.Driver.UpdateDefinition<T>> update,
            MongoDB.Driver.UpdateOptions options = null)
        {
            var mongoDbOperation = new MongoDbUpdateOperation<T>(new CommandActivator(), database, collectionName, filter, update, options);
            observed.Subscribe(mongoDbOperation);
            return mongoDbOperation;
        }
    }
}