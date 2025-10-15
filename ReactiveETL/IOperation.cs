namespace ReactiveETL;

using System;
using System.Collections.Generic;

/// <summary>
/// Service contract for operations
/// </summary>
public interface IOperation : IObservableOperation, IObserver<Row>
{
    /// <summary>
    /// List of operation observed by this operation
    /// </summary>
    List<IObservableOperation> Observed { get; }
}