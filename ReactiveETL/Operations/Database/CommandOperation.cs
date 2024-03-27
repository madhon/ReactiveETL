using System;
using ReactiveETL.Activators;

namespace ReactiveETL.Operations.Database
{
    using System.Data;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Operation that apply a database command. If this operation is a starting point (it does not observe anything), calling the process method will execute the command and start the pipeline.
    /// </summary>
    public partial class CommandOperation : AbstractOperation
    {
        private readonly ILogger log;

        private readonly CommandActivator _activator;

        /// <summary>
        /// Command operation constructor
        /// </summary>
        /// <param name="activator">command parameters</param>
        public CommandOperation(CommandActivator activator, ILogger logger)
        {
            _activator = activator;
            this.log = logger;
        } 

        /// <summary>
        /// Notifies the observer of a new value in the sequence. It's best to override Dispatch or TreatRow than this method because this method contains pipeline logic.
        /// </summary>
        public override void OnNext(Row value)
        {
            CountTreated++;
            
            _activator.UseCommand(currentCommand =>
            {
                _activator.Prepare?.Invoke(currentCommand, value);

                LogDisplayNameCommandText(DisplayName, currentCommand.CommandText);

                if (_activator.IsQuery)
                {
                    using (IDataReader reader = currentCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                value.Add(reader);                                
                            }
                            catch (Exception ex)
                            {
                                if (_activator.FailOnError)
                                    throw;

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

            base.OnNext(value);
        }

        /// <summary>
        /// Notifies the observers of the end of the sequence.
        /// </summary>
        public override void OnCompleted()
        {
            _activator.Release();
            base.OnCompleted();
        }
        
        [LoggerMessage(
            1000,
            LogLevel.Warning,
            "Non blocking operation error",
            EventName = "LogNonBlockingError")]
        private partial void LogNonBlockingError(Exception ex);
        
        [LoggerMessage(
            1002,
            LogLevel.Information,
            "{displayName} Execute command {commandText}",
            EventName = "LogDisplayNameCommandText")]
        private partial void LogDisplayNameCommandText(string displayName, string commandText);
        
    }
}