namespace ReactiveETL;

using System;
using System.Collections.Generic;
using System.Threading;

/// <summary>
/// Encapsulation of information for the result of a pipeline
/// </summary>
public class EtlResult
{
    internal DateTime _processStart = DateTime.Now;
    internal DateTime? _processEnd;
    internal List<Exception> _exceptions = new List<Exception>();

    /// <summary>
    /// Indicate if the pipeline process is over
    /// </summary>
    public bool Completed
    {
        get;
        internal set;
    }

    /// <summary>
    /// Thread in witch the pipeline is running, null value means current thread
    /// </summary>
    public Thread Thread
    {
        get;
        internal set;
    }

    /// <summary>
    /// Elements recorded by the operation
    /// </summary>
    public int CountExceptions => _exceptions.Count;

    /// <summary>
    /// Exceptions recorded by the operation
    /// </summary>
    public IEnumerable<Exception> Exceptions => _exceptions;

    /// <summary>
    /// Duration of the process
    /// </summary>
    public TimeSpan? Duration
    {
        get
        {
            if (_processEnd.HasValue)
            {
                return _processEnd.Value - _processStart;
            }
            return null;
        }
    }
}