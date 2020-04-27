using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ReactiveETL
{
    /// <summary>
    /// Base class for operations
    /// </summary>
    [DebuggerDisplay("{DisplayName}")]
    public abstract class AbstractOperation : AbstractObservableOperation, IOperation
    {
        private List<IObservableOperation> _observed = new List<IObservableOperation>();



        /// <summary>
        /// List of operation observed by this operation
        /// </summary>
        public List<IObservableOperation> Observed => _observed;


        /// <summary>
        /// Notifies the observer of a new value in the sequence. It's best to override Dispatch or TreatRow than this method because this method contains pipeline logic.
        /// </summary>
        public virtual void OnNext(Row value)
        {
            if (value == null)
                throw new ArgumentNullException("Row could not be null");

            CountTreated++;

            Dispatch(value);
        }

        /// <summary>
        /// Method called by OnNext to dispatch the new value to the observers of the operation
        /// </summary>
        /// <param name="value">value to dispatch</param>
        protected virtual void Dispatch(Row value)
        {
            var processed = TreatRow(value);
            if (processed != null)
            {
                Observers.PropagateOnNext(processed);
            }
        }

        /// <summary>
        /// Method called by OnNext > Dispatch to process the notified value. 
        /// This method just return the value and could be overridden in subclasses.
        /// Return null if you want to skip the processed row
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual Row TreatRow(Row value)
        {
            return value;
        }

        /// <summary>
        /// Notifies the observer that an exception has occurred.
        /// </summary>
        public virtual void OnError(Exception exception)
        {
            Observers.PropagateOnError(exception);
        }

        /// <summary>
        /// Notifies the observers of the end of the sequence.
        /// </summary>
        public virtual void OnCompleted()
        {
            // We should consider this operation completed only if all "parent" operations completes
            if (Observed.All(o => o.Completed))
            {
                Completed = true;
                Observers.PropagateOnCompleted();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            Observed.DisposeAll();
        }

        /// <summary>
        /// Trigger the operation. Trigger method calls are bubbled up through the pipeline
        /// </summary>
        public override void Trigger()
        {
            if (Observed.Count == 0)
            {
                OnNext(null);
            }
            else
            {
                Observed.TriggerAll();
            }
        }
    }
}