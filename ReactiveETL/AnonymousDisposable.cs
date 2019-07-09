namespace ReactiveETL
{
    using System;

    class AnonymousDisposable : IDisposable
    {
        readonly Action dispose;

        public AnonymousDisposable(Action dispose) => this.dispose = dispose;

        public void Dispose() => dispose();
    } 
}
