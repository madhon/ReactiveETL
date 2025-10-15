namespace ReactiveETL;

using System;
using System.Collections.Generic;

internal static class Helpers
{
    public static void PropagateOnCompleted(this IEnumerable<IObserver<Row>> observers)
    {
        foreach (var observer in observers)
        {
            observer.OnCompleted();
        }
    }

    public static void PropagateOnNext(this IEnumerable<IObserver<Row>> observers, Row row)
    {
        foreach (var observer in observers)
        {
            observer.OnNext(row);
        }
    }

    public static void PropagateOnError(this IEnumerable<IObserver<Row>> observers, Exception ex)
    {
        foreach (var observer in observers)
        {
            observer.OnError(ex);
        }
    }

    public static void DisposeAll(this IEnumerable<IObservableOperation> allobserved)
    {
        foreach (var observed in allobserved)
        {
            observed.Dispose();
        }
    }

    public static void TriggerAll(this IEnumerable<IObservableOperation> observed)
    {
        foreach (var elt in observed)
        {
            elt.Trigger();
        }
    }
}