using System;
using ReactiveETL.Activators;
using ReactiveETL.Logging;

namespace ReactiveETL.Operations.Database
{
    using System.Data;

    /// <summary>
    /// Operation that apply a database command. If this operation is a starting point (it does not observe anything), calling the process method will execute the command and start the pipeline.
    /// </summary>
    public class CommandOperation : AbstractOperation
    {
        private readonly ILog log = LogProvider.GetCurrentClassLogger();

        private CommandActivator _activator;

        /// <summary>
        /// Command operation constructor
        /// </summary>
        /// <param name="activator">command parameters</param>
        public CommandOperation(CommandActivator activator)
        {
            _activator = activator;
        }

        /// <summary>
        /// Notifies the observer of a new value in the sequence. It's best to override Dispatch or TreatRow than this method because this method contains pipeline logic.
        /// </summary>
        public override void OnNext(Row value)
        {
            CountTreated++;
            
            _activator.UseCommand(currentCommand =>
            {
                if (_activator.Prepare != null)
                    _activator.Prepare(currentCommand, value);

                log.Info(DisplayName + " Execute command " + currentCommand.CommandText);

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

                                log.WarnException("Non blocking operation error", ex);
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
    }
}