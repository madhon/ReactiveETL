using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReactiveETL.Operations
{
    /// <summary>
    /// Operation to apply an action from the rows
    /// </summary>
    public class ApplyOperation : AbstractOperation
    {
        private Action<Row> _rowAction;

        /// <summary>
        /// Constructor of the action
        /// </summary>
        /// <param name="rowAction">callback method to the action</param>
        public ApplyOperation(Action<Row> rowAction)
        {
            _rowAction = rowAction;
        }

        /// <summary>
        /// Method called by OnNext > Dispatch to process the notified value. This method just return the value and could be overriden in subclasses
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override Row TreatRow(Row value)
        {
            _rowAction(value);
            return base.TreatRow(value);
        }
    }
}
