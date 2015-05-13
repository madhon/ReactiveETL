using System.Collections.Generic;

namespace ReactiveETL.Operations
{
    /// <summary>
    /// Operation to dispatch grouped elements
    /// </summary>
    public class DispatchGroupOperation : AbstractOperation
    {
        /// <summary>
        /// Method called by OnNext to dispatch the new value to the observers of the operation
        /// </summary>
        /// <param name="value">value to dispatch</param>
        protected override void Dispatch(Row value)
        {
            var grouped = (List<Row>)value[Constants.GroupListName];
            foreach (var row in grouped)
            {
                row[Constants.GroupParentName] = value;
                base.Dispatch(row);                
            }
        }
    }
}
