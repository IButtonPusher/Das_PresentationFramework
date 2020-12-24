using System;
using System.Threading.Tasks;

namespace System.Threading
{
    public class SyncReader : IDisposable
    {
        public SyncReader(Action disposeAction)
        {
            _disposeAction = disposeAction;
        }

        public void Dispose()
        {
            _disposeAction();
        }

        private readonly Action _disposeAction;
    }
}