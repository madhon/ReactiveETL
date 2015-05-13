using System;

namespace ReactiveETL.Operations
{
    /// <summary>
    /// Operation used to record minimum run informations
    /// </summary>
    public class StartOperation : AbstractOperation
    {
        /// <summary>
        /// Constructor of start operation
        /// </summary>
        public StartOperation()
        {
            Result = new EtlResult();
        }

        /// <summary>
        /// Result of the pipeline's process
        /// </summary>
        public EtlResult Result
        {
            get; protected set;
        }

        /// <summary>
        /// Notifies the observer that an exception has occurred.
        /// </summary>
        public override void OnError(Exception exception)
        {
            Result._exceptions.Add(exception);
            base.OnError(exception);
        }

        /// <summary>
        /// Trigger the operation. Trigger method calls are bubbled up through the pipeline
        /// </summary>
        public override void Trigger()
        {
            Result._processStart = DateTime.Now;
            base.Trigger();
        }

        /// <summary>
        /// Notifies the observer of the end of the sequence.
        /// </summary>
        public override void OnCompleted()
        {
            if (!Result._processEnd.HasValue)
                Result._processEnd = DateTime.Now;
            Result.Completed = true;
            base.OnCompleted();
        }
    }
}
