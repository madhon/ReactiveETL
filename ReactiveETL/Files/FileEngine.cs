
namespace ReactiveETL.Files;

using System.Collections.ObjectModel;
using System;
using System.Collections;
using FileHelpers;


/// <summary>
/// Adapter class to facilitate the nicer syntax
/// </summary>
public sealed class FileEngine :  CollectionBase, IDisposable
{
    private readonly FileHelperAsyncEngine engine;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileEngine"/> class.
    /// </summary>
    /// <param name="engine">The engine.</param>
    public FileEngine(FileHelperAsyncEngine engine) => this.engine = engine;

    /// <summary>
    /// Writes the specified object to the file
    /// </summary>
    /// <param name="t">The t.</param>
    public void Write(object t) => engine.WriteNext(t);

    /// <summary>
    /// Set the behaviour on error
    /// </summary>
    /// <param name="errorMode">The error mode.</param>
    public FileEngine OnError(ErrorMode errorMode)
    {
        engine.ErrorMode = errorMode;
        return this;
    }

    /// <summary>
    /// Gets a value indicating whether this instance has errors.
    /// </summary>
    public bool HasErrors => engine.ErrorManager.HasErrors;

    /// <summary>
    /// Outputs the errors to the specified file
    /// </summary>
    /// <param name="file">The file.</param>
    public void OutputErrors(string file)
    {
        engine.ErrorManager.SaveErrors(file);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        IDisposable d = engine;
        d.Dispose();
    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    /// An <see cref="System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
    /// </returns>
    public new IEnumerator GetEnumerator()
    {
        IEnumerable e = engine;
        return e.GetEnumerator();
    }
}