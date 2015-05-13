using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReactiveETL
{
    class AnonymousDisposable : IDisposable
    {
        Action dispose;

        public AnonymousDisposable(Action dispose)
        {
            this.dispose = dispose;
        }

        public void Dispose()
        {
            dispose();
        }
    } 
}
