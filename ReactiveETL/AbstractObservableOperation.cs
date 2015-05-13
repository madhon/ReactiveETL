using System;
using System.Collections.Generic;

namespace ReactiveETL
{
    /// <summary>
    /// Base class for observable operations
    /// </summary>
    public class AbstractObservableOperation : IObservableOperation
    {
        private List<IObserver<Row>> _observers = new List<IObserver<Row>>();

        /// <summary>
        /// Number of elements treated
        /// </summary>
        public int CountTreated
        {
            get;
            protected set;
        }

        /// <summary>
        /// Indicate if the operation is over
        /// </summary>
        public bool Completed { get; protected set; }

        /// <summary>
        /// Name of the operation
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Display name of the operation. Return the Name if available or GetType().Name otherwise
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (!string.IsNullOrEmpty(Name))
                    return Name;

                return GetType().Name;
            }
        }

        /// <summary>
        /// List of observers of this operation
        /// </summary>
        public List<IObserver<Row>> Observers
        {
            get
            {
                return _observers;
            }
        }

        #region Observable

        /// <summary>
        /// Subscribes an observer to the observable sequence.
        /// </summary>
        public virtual IDisposable Subscribe(IObserver<Row> observer)
        {
            if (observer is IOperation)
            {
                ((IOperation)observer).Observed.Add(this);
            }
            _observers.Add(observer);
            return new AnonymousDisposable(() => _observers.Remove(observer));
        }
        #endregion

        /// <summary>
        /// Trigger the operation. Trigger method calls are bubbled up through the pipeline
        /// </summary>
        public virtual void Trigger()
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public virtual void Dispose()
        {            
        }
    }
}
