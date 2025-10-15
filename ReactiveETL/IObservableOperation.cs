using System;
using System.Collections.Generic;

namespace ReactiveETL
{
    /// <summary>
    /// Service contract for input operation
    /// </summary>
    public interface IObservableOperation : IObservable<Row>, IDisposable
    {
        /// <summary>
        /// Name of the operation
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Indicate if the operation is over
        /// </summary>
        bool Completed { get; }

        /// <summary>
        /// List of observers of this operation
        /// </summary>
        IList<IObserver<Row>> Observers { get; }

        /// <summary>
        /// Start the operation. Start method calls are bubbled up through the pipeline
        /// </summary>
        void Trigger();
    }
}