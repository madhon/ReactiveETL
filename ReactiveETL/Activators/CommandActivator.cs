using System;
using System.Data;
using ReactiveETL.Infrastructure;

namespace ReactiveETL.Activators
{
    /// <summary>
    /// Class to manage command parameters and execution
    /// </summary>
    public class CommandActivator
    {
        /// <summary>
        /// Name of a connection string defined in the application configuration file
        /// </summary>
        public string ConnStringName { get; set; }

        /// <summary>
        /// Text of the command
        /// </summary>
        public string CommandText { get; set; }

        /// <summary>
        /// Indicate if the process should use a transaction
        /// </summary>
        public bool UseTransaction { get; set; }

        private bool failOnError = true;
        /// <summary>
        /// Indicate if operation fail on error (default is true)
        /// </summary>
        public bool FailOnError
        {
            get
            {
                return this.failOnError;
            }
            set
            {
                this.failOnError = value;
            }
        }

        /// <summary>
        /// Indicate if the command is a query
        /// </summary>
        public bool IsQuery { get; set; }

        /// <summary>
        /// Database connection
        /// </summary>
        public IDbConnection Connection { get; set; }

        /// <summary>
        /// Database connection
        /// </summary>
        public IDbTransaction Transaction { get; set; }


        private bool _selfCreatedConnection;
        private bool _rolled;
        private IDbCommand _currentCommand;

        /// <summary>
        /// Callback method for preparing the command
        /// </summary>
        public Action<IDbCommand, Row> Prepare { get; set; }

        private void CheckConnection()
        {
            if (Connection == null)
            {
                _selfCreatedConnection = true;
                Connection = Use.Connection(ConnStringName);
                if (UseTransaction)
                {
                    Transaction = Connection.BeginTransaction();
                }
            }
        }

        /// <summary>
        /// Create a row from the IDataReader
        /// </summary>
        /// <param name="reader">IDataReader to convert</param>
        /// <returns>row initialized with IDataReader values</returns>
        public virtual Row CreateRowFromReader(IDataReader reader)
        {
            return Row.FromReader(reader);
        }

        /// <summary>
        /// Provide a command and take care of cleaning up
        /// </summary>
        /// <param name="usecmd"></param>
        public void UseCommand(Action<IDbCommand> usecmd)
        {
            CheckConnection();
            using (_currentCommand = Connection.CreateCommand())
            {
                _currentCommand.Transaction = Transaction;
                _currentCommand.CommandText = CommandText;
                usecmd(_currentCommand);
                _currentCommand = null;
            }
        }

        /// <summary>
        /// Release the plumbing (connection, transaction, etc)
        /// </summary>
        public void Rollback()
        {
            if (_selfCreatedConnection)
            {
                if (UseTransaction)
                {
                    Transaction.Rollback();
                    _rolled = true;
                }
            }
        }

        /// <summary>
        /// Release the plumbing (connection, transaction, etc)
        /// </summary>
        public void Release()
        {
            if (_selfCreatedConnection)
            {
                if (UseTransaction && !_rolled)
                {
                    Transaction.Commit();
                }
                if (Connection != null)
                {
                    Connection.Close();
                    Connection.Dispose();
                }
            }
        }
    }
}