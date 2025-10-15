namespace ReactiveETL.Activators;

using System.Collections;
using System.Collections.Generic;
using ReactiveETL.Operations;

/// <summary>
/// Join activator for an operation. The first time the activator is iterated will trigger the operation pipeline.
/// </summary>
public class OperationJoinActivator : JoinActivator<Row>, IEnumerable<Row>
{
    /// <summary>
    /// Joined operation
    /// </summary>
    public IObservableOperation Operation { get; set; }

    private RecordOperation _recorded;

    private void CheckOperation()
    {
        if (_recorded == null)
        {
            // To save memory, if the last operation is a record, we do not create a new one.
            if (Operation is RecordOperation)
            {
                _recorded = (RecordOperation)Operation;
            }
            else
            {
                _recorded = Operation.Record();
            }
            _recorded.Start();
        }
    }

    #region IEnumerable<T> Members
    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// A <see cref="System.Collections.Generic.IEnumerator"/> that can be used to iterate through the collection.
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public override IEnumerator<Row> GetEnumerator()
    {
        CheckOperation();
        return _recorded.Result.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    /// An <see cref="System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    IEnumerator IEnumerable.GetEnumerator()
    {
        CheckOperation();
        return _recorded.Result.GetEnumerator();
    }
    #endregion
}