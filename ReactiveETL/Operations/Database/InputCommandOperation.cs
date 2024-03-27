using System;
using System.Data;
using ReactiveETL.Activators;

namespace ReactiveETL.Operations.Database
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// Observable list of elements
    /// </summary>
    public partial class InputCommandOperation : AbstractObservableOperation
    {
        private readonly ILogger log;

        private CommandActivator _activator;

        /// <summary>
        /// Constructor of input command operation
        /// </summary>
        /// <param name="activator"></param>
        public InputCommandOperation(CommandActivator activator, ILogger log)
        {
            _activator = activator;
            this.log = log;

        }

        /// <summary>
        /// Notifies the observer of a new value in the sequence. It's best to override Dispatch or TreatRow than this method because this method contains pipelining logic.
        /// </summary>
        public override void Trigger()
        {
            try
            {
                _activator.UseCommand(currentCommand =>
                {
                    if (_activator.Prepare != null)
                    {
                        _activator.Prepare(currentCommand, null);
                    }

                    LogDisplayNameCommandText(DisplayName, currentCommand.CommandText);

                    if (_activator.IsQuery)
                    {
                        using (IDataReader reader = currentCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                try
                                {
                                    Observers.PropagateOnNext(_activator.CreateRowFromReader(reader));
                                }
                                catch (Exception ex)
                                {
                                    if (_activator.FailOnError)
                                    {
                                        throw;
                                    }

                                    LogNonBlockingError(ex);
                                }
                            }

                        }
                    }
                    else
                    {
                        currentCommand.ExecuteNonQuery();
                    }
                });
            }
            catch (Exception ex)
            {
                LogOperationError(ex);
                Observers.PropagateOnError(ex);
            }
            finally
            {
                _activator.Release();
            }
            
            Completed = true;
            Observers.PropagateOnCompleted();
        }
        
        [LoggerMessage(
            1000,
            LogLevel.Warning,
            "Non blocking operation error",
            EventName = "LogNonBlockingError")]
        private partial void LogNonBlockingError(Exception ex);
        
        [LoggerMessage(
            1001,
            LogLevel.Error,
            "Operation error",
            EventName = "Operation error")]
        partial void LogOperationError(Exception ex);
        
        [LoggerMessage(
            1002,
            LogLevel.Information,
            "{displayName} Execute command {commandText}",
            EventName = "LogDisplayNameCommandText")]
        private partial void LogDisplayNameCommandText(string displayName, string commandText);
    }
}
