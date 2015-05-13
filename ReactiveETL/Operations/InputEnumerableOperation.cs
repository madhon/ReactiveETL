using System;
using System.Collections.Generic;

namespace ReactiveETL.Operations
{
    /// <summary>
    /// Operation used to create an operation from an enumeration
    /// </summary>
    public class InputEnumerableOperation<T> : AbstractObservableOperation where T : class
    {
        private IEnumerable<T> _enumeration;

        /// <summary>
        /// Enumerable operation constructor
        /// </summary>
        /// <param name="enumeration">enumeration to use as data input</param>
        public InputEnumerableOperation(IEnumerable<T> enumeration)
        {
            _enumeration = enumeration;
        }

        /// <summary>
        /// Notifies the observer of a new value in the sequence. It's best to override Dispatch or TreatRow than this method because this method contains pipelining logic.
        /// </summary>
        public override void Trigger()
        {
            CountTreated++;
            try
            {
                foreach (var elt in _enumeration)
                {
                    if (elt is Row)
                        Observers.PropagateOnNext(elt as Row);
                    else
                        Observers.PropagateOnNext(Row.FromObject(elt));
                }
                
            }
            catch (Exception ex)
            {
                Observers.PropagateOnError(ex);
            }           
            
            Completed = true;
            Observers.PropagateOnCompleted(); 
        }
    }
}