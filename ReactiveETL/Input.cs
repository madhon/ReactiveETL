namespace ReactiveETL;

using System;
using System.Data;
using System.IO;
using ReactiveETL.Activators;
using ReactiveETL.Operations;
using ReactiveETL.Operations.Database;
using ReactiveETL.Operations.File;
using System.Collections.Generic;

/// <summary>
/// Helper call for starting a pipeline process
/// </summary>
public static class Input
{
    /// <summary>
    /// Apply a database query command
    /// </summary>
    /// <param name="connStr">Name of a connection string defined in the application configuration file</param>
    /// <param name="commandText">Text of the command</param>
    /// <returns>command operation</returns>
    public static InputCommandOperation Query(string connStr, string commandText)
    {
        return Command(connStr, commandText, true, true, null);
    }

    /// <summary>
    /// Apply a database query command
    /// </summary>
    /// <param name="connStr">Name of a connection string defined in the application configuration file</param>
    /// <param name="commandText">Text of the command</param>
    /// <param name="failOnError">Indicate if the operation must fail on element error</param>
    /// <returns>command operation</returns>
    public static InputCommandOperation Query(string connStr, string commandText, bool failOnError)
    {
        return Command(connStr, commandText, true, failOnError, null);
    }

    /// <summary>
    /// Apply a database query command
    /// </summary>
    /// <param name="connection">Database connection</param>
    /// <param name="commandText">Text of the command</param>
    /// <returns>command operation</returns>
    public static InputCommandOperation Query(IDbConnection connection, string commandText)
    {
        return Command(connection, commandText, true, true, null);
    }

    /// <summary>
    /// Apply a database query command
    /// </summary>
    /// <param name="connection">Database connection</param>
    /// <param name="commandText">Text of the command</param>
    /// <param name="failOnError">Indicate if the operation must fail on element error</param>
    /// <returns>command operation</returns>
    public static InputCommandOperation Query(IDbConnection connection, string commandText, bool failOnError)
    {
        return Command(connection, commandText, true, failOnError, null);
    }

    /// <summary>
    /// Apply a database non query command
    /// </summary>
    /// <param name="connStr">Name of a connection string defined in the application configuration file</param>
    /// <param name="commandText">Text of the command</param>
    /// <returns>command operation</returns>
    public static InputCommandOperation NonQuery(string connStr, string commandText)
    {
        return Command(connStr, commandText, false, true, null);
    }

    /// <summary>
    /// Apply a database non query command
    /// </summary>
    /// <param name="connStr">Name of a connection string defined in the application configuration file</param>
    /// <param name="commandText">Text of the command</param>
    /// <returns>command operation</returns>
    public static void RunNonQuery(string connStr, string commandText)
    {
        var op = Command(connStr,commandText, false, true, null);
        op.Execute(true);
    }

    /// <summary>
    /// Apply a database non query command
    /// </summary>
    /// <param name="connection">Database connection</param>
    /// <param name="commandText">Text of the command</param>
    /// <returns>command operation</returns>
    public static InputCommandOperation NonQuery(IDbConnection connection, string commandText)
    {
        return Command(connection, commandText, false, true, null);
    }

    /// <summary>
    /// Apply a database command
    /// </summary>
    /// <param name="activator">command parameters</param>
    /// <returns>command operation</returns>
    public static InputCommandOperation Command(CommandActivator activator)
    {
        var cmd = new InputCommandOperation(activator, LogProvider.GetLogger(typeof(InputCommandOperation).ToString()));
        return cmd;
    }

    /// <summary>
    /// Apply a database command
    /// </summary>
    /// <param name="connStr">Name of a connection string defined in the application configuration file</param>
    /// <param name="commandText">Text of the command</param>
    /// <param name="isQuery">Indicate if the command is a query</param>
    /// <param name="failOnError">Indicate if the operation must fail on element error</param>
    /// <param name="prepare">Callback method to prepare the command</param>
    /// <returns>Command operation</returns>
    public static InputCommandOperation Command(string connStr, string commandText, bool isQuery, bool failOnError, Action<IDbCommand, Row> prepare)
    {
        var activator = new CommandActivator();
        activator.ConnStringName = connStr;
        activator.CommandText = commandText;
        activator.Prepare = prepare;
        activator.IsQuery = isQuery;
        activator.FailOnError = failOnError;

        return Command(activator);
    }

    /// <summary>
    /// Apply a database command
    /// </summary>
    /// <param name="connection">Database connection</param>
    /// <param name="commandText">Text of the command</param>
    /// <param name="isQuery">Indicate if the command is a query</param>
    /// <param name="failOnError">Indicate if the operation must fail on element error</param>
    /// <param name="prepare">Callback method to prepare the command</param>
    /// <returns>Command operation</returns>
    public static InputCommandOperation Command(IDbConnection connection, string commandText, bool isQuery, bool failOnError, Action<IDbCommand, Row> prepare)
    {
        var activator = new CommandActivator();
        activator.Connection = connection;
        activator.CommandText = commandText;
        activator.Prepare = prepare;
        activator.IsQuery = isQuery;
        activator.FailOnError = failOnError;

        return Command(activator);
    }

    /// <summary>
    /// Input data from a file
    /// </summary>
    /// <typeparam name="T">type of the object used to read the file content</typeparam>
    /// <param name="filename">full path to the file</param>
    /// <returns>file read operation</returns>
    public static InputFileOperation<T> ReadFile<T>(string filename)
    {
        return new InputFileOperation<T>(filename, LogProvider.GetLogger(typeof(InputCommandOperation).ToString()));
    }

    /// <summary>
    /// Input data from a file
    /// </summary>
    /// <typeparam name="T">type of the object used to read the file content</typeparam>
    /// <param name="strm">file stream</param>
    /// <returns>file read operation</returns>
    public static InputFileOperation<T> ReadFile<T>(Stream strm)
    {
        return new InputFileOperation<T>(strm);
    }

    /// <summary>
    /// Input data from a file
    /// </summary>
    /// <typeparam name="T">type of the object used to read the file content</typeparam>
    /// <param name="reader">file stream</param>
    /// <returns>file read operation</returns>
    public static InputFileOperation<T> ReadFile<T>(StreamReader reader)
    {
        return new InputFileOperation<T>(reader);
    }

    /// <summary>
    /// Input data from an enumerable
    /// </summary>
    /// <typeparam name="T">type of the object used in the enumerable</typeparam>
    /// <param name="source">data source</param>
    /// <returns>enumerable operation</returns>
    public static InputEnumerableOperation<T> From<T>(IEnumerable<T> source) where T : class
    {
        return new InputEnumerableOperation<T>(source);
    }
}