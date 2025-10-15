namespace ReactiveETL;

using System.Collections.Generic;

/// <summary>
/// Result of a pipeline
/// </summary>
public class EtlFullResult : EtlResult, IEnumerable<Row>
{
    internal EtlFullResult()
    {
            
    }

    internal List<Row> _data = [];

    /// <summary>
    /// Elements recorded by the operation
    /// </summary>
    public IEnumerable<Row> Data => _data;

    /// <summary>
    /// Elements recorded by the operation
    /// </summary>
    public int Count => _data.Count;

    #region IEnumerable<Row> Members

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// A <see cref="System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public IEnumerator<Row> GetEnumerator()
    {
        return _data.GetEnumerator();
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
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return _data.GetEnumerator();
    }

    #endregion
}