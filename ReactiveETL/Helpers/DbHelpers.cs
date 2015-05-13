using System;
using System.Data;

namespace ReactiveETL
{
    /// <summary>
    /// Helper methods for database manipulations
    /// </summary>
    public static class DbHelpers
    {
        /// <summary>
        /// Adds the parameter the specifed command
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="name">The name.</param>
        /// <param name="val">The val.</param>
        public static void AddParameter(this IDbCommand command, string name, object val)
        {
            IDbDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = val ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }
    }
}
