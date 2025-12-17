namespace ReactiveETL;

using System;
using System.Collections.ObjectModel;

/// <summary>
/// Service contract for operations
/// </summary>
public interface IOperation : IObservableOperation, IObserver<Row>
{
    /// <summary>
    /// List of operation observed by this operation
    /// </summary>
    Collection<IObservableOperation> Observed { get; }
}