namespace ReactiveETL.Exceptions;

using System;
using System.Runtime.Serialization;

/// <summary>
/// An exception that was caught during exceuting the code.
/// </summary>
[Serializable]
public class ReactiveETLException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveETLException"/> class.
    /// </summary>        
    public ReactiveETLException()
        : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveETLException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="inner">The inner.</param>
    public ReactiveETLException(string message, Exception inner) : base(message, inner)
    {
    }
}