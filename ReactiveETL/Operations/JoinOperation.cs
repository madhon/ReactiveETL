using System.Linq;
using ReactiveETL.Activators;
using System.Collections.Generic;

namespace ReactiveETL.Operations
{
    /// <summary>
    /// Operation for joining data in the pipeline
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JoinOperation<T> : AbstractOperation
    {
        private Dictionary<T, object> matchedRightElt = new Dictionary<T, object>();
        private JoinActivator<T> _activator;

        /// <summary>
        /// Join operation constructor
        /// </summary>
        /// <param name="activator">Join operation activator</param>
        public JoinOperation(JoinActivator<T> activator)
        {
            _activator = activator;
        }

        /// <summary>
        /// Method called by OnNext > Dispatch to process the notified value. This method just return the value and could be overriden in subclasses
        /// </summary>
        /// <param name="value">pipelined value</param>
        /// <returns>Processed row</returns>
        protected override Row TreatRow(Row value)
        {
            T listElement = _activator.FirstOrDefault(elt => _activator.CheckMatch(value, elt));            

            if (_activator.CheckMatch(value, listElement))
            {
                if (listElement != null)
                    matchedRightElt[listElement] = null;
                var res = _activator.ProcessRow(value, listElement);
                return base.TreatRow(res);
            }

            return null;
        }

        /// <summary>
        /// Notifies the observers of the end of the sequence.
        /// </summary>
        public override void OnCompleted()
        {
            foreach (var elt in _activator)
            {
                if (matchedRightElt.ContainsKey(elt))
                    continue;

                Row emptyRow = new Row();
                if (_activator.CheckMatch(emptyRow, elt))
                {
                    var res = _activator.ProcessRow(emptyRow, elt);
                    Observers.PropagateOnNext(res);
                }
            }
            base.OnCompleted();
        }
    }
}