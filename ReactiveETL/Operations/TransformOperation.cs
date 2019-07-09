using System;
using System.Collections.Generic;

namespace ReactiveETL.Operations
{
    /// <summary>
    /// Operation for transforming the current <see cref="Row"/>
    /// </summary>
    public class TransformOperation : AbstractOperation
    {
        private Func<Row, Row> _transformact;
        private Func<Row, IEnumerable<Row>> _transformManyAct;

        /// <summary>
        /// Transform operation constructor
        /// </summary>
        /// <param name="transformact">callback method for transforming the <see cref="Row"/></param>
        public TransformOperation(Func<Row, Row> transformact) => _transformact = transformact;

        /// <summary>
        /// Transform operation constructor
        /// </summary>
        /// <param name="transformact">callback method for transforming the <see cref="Row"/></param>
        public TransformOperation(Func<Row, IEnumerable<Row>> transformact) => _transformManyAct = transformact;

        /// <summary>
        /// Method called by OnNext to dispatch the new value to the observers of the operation
        /// </summary>
        /// <param name="value">value to dispatch</param>
        protected override void Dispatch(Row value)
        {
            if (_transformact != null)
            {
                base.Dispatch(_transformact(value));
            }
            else if (_transformManyAct != null)
            {
                var res = _transformManyAct(value);
                foreach (var row in res)
                {
                    base.Dispatch(row);
                }
            }
        }
    }
}