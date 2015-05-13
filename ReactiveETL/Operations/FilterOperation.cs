using System;

namespace ReactiveETL.Operations
{
    /// <summary>
    /// Operation to filter data in the pipeline
    /// </summary>
    public class FilterOperation : AbstractOperation
    {
        private Predicate<Row> _predicate;

        /// <summary>
        /// Filter operation constructor
        /// </summary>
        /// <param name="predicate">predicate applyied to filter data</param>
        public FilterOperation(Predicate<Row> predicate)
        {
            _predicate = predicate;
        }


        /// <summary>
        /// Dispatch the value only if filter condition is met
        /// </summary>
        /// <param name="value">value to dispatch</param>
        protected override void Dispatch(Row value)
        {
            if (_predicate(value))
                base.Dispatch(value);
        }
    }
}