﻿namespace ReactiveETL;

using System;
using System.Data;
using ReactiveETL.Activators;
using ReactiveETL.Operations.Database;

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
        var cmd = new CommandOperation(activator, LogProvider.GetLogger(typeof(CommandOperation).ToString()));
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
        var cmd = new CommandOperation(activator, LogProvider.GetLogger(typeof(CommandOperation).ToString()));
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
    public static CommandOperation DbCommand(this IObservableOperation observed, string connStr, string CommandText) => observed.DbCommand(connStr, CommandText, false, null);

    /// <summary>
    /// Apply a command operation
    /// </summary>
    /// <param name="observed">observed operation</param>
    /// <param name="connection">Database connection</param>
    /// <param name="CommandText">text of the command</param>
    /// <returns>command operation</returns>
    public static CommandOperation DbCommand(this IObservableOperation observed, IDbConnection connection, string CommandText) => observed.DbCommand(connection, CommandText, false, null);

    /// <summary>
    /// Apply a command operation
    /// </summary>
    /// <param name="observed">observed operation</param>
    /// <param name="connStr">name of a connection string defined in the application configuration file</param>
    /// <param name="prepare">callback method to prepare the command</param>
    /// <returns>command operation</returns>
    public static CommandOperation DbCommand(this IObservableOperation observed, string connStr, Action<IDbCommand, Row> prepare) => observed.DbCommand(connStr, null, false, prepare);

    /// <summary>
    /// Apply a command operation
    /// </summary>
    /// <param name="observed">observed operation</param>
    /// <param name="connection">Database connection</param>
    /// <param name="prepare">callback method to prepare the command</param>
    /// <returns>command operation</returns>
    public static CommandOperation DbCommand(this IObservableOperation observed, IDbConnection connection, Action<IDbCommand, Row> prepare) => observed.DbCommand(connection, null, false, prepare);

    /// <summary>
    /// Apply a command operation
    /// </summary>
    /// <param name="observed">observed operation</param>
    /// <param name="connStr">Name of a connection string defined in the application configuration file</param>
    /// <param name="CommandText">text of the command</param>
    /// <param name="prepare">callback method to prepare the command</param>
    /// <returns>command operation</returns>
    public static CommandOperation DbCommand(this IObservableOperation observed, string connStr, string CommandText, Action<IDbCommand, Row> prepare) => observed.DbCommand(connStr, null, false, prepare);

    /// <summary>
    /// Apply a command operation
    /// </summary>
    /// <param name="observed">observed operation</param>
    /// <param name="connection">Database connection</param>
    /// <param name="CommandText">text of the command</param>
    /// <param name="prepare">callback method to prepare the command</param>
    /// <returns>command operation</returns>
    public static CommandOperation DbCommand(this IObservableOperation observed, IDbConnection connection, string CommandText, Action<IDbCommand, Row> prepare) => observed.DbCommand(connection, null, false, prepare);

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
        CommandActivator activator = new ();

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
        CommandActivator activator = new ();

        activator.ConnStringName = connStr;
        activator.CommandText = CommandText;
        activator.Prepare = prepare;
        activator.IsQuery = isQuery;

        return observed.DbCommand(activator);
    }
}