using System.Collections.Generic;
using System;

namespace ReactiveETL.Operations
{
    /// <summary>
    /// Operation that record values in a list while continuing the pipeline
    /// </summary>
    public class RecordOperation : StartOperation
    {
        /// <summary>
        /// Constructor of the operation
        /// </summary>
        public RecordOperation()
        {
            this.Result = new EtlFullResult();
            base.Result = this.Result;
        }
        
        /// <summary>
        /// Elements recorded by the operation
        /// </summary>
        public new EtlFullResult Result
        {
            get; protected set;
        }

        /// <summary>
        /// Notifies the observer of a new value in the sequence. It's best to override Dispatch or TreatRow than this method because this method contains pipelining logic.
        /// </summary>
        public override void OnNext(Row value)
        {
            Result._data.Add(value);
            base.OnNext(value);
        }
    }
}