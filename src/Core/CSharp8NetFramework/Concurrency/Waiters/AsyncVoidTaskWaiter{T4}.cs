using System;
using System.Threading.Tasks;

namespace System.Threading
{
    public class AsyncVoidTaskWaiter<TParam1, TParam2, TParam3, TParam4> : AsyncVoidTaskWaiterBase
    {
        public AsyncVoidTaskWaiter(TParam1 p1,
                                   TParam2 p2,
                                   TParam3 p3,
                                   TParam4 p4,
                                   Func<TParam1, TParam2, TParam3, TParam4, Task> action,
                                   WorkerTypes workerType)
            : base(workerType, action)
        {
            _p1 = p1;
            _p2 = p2;

            _p3 = p3;
            _p4 = p4;

            _action = action;
        }

        public override async Task ExecuteAsync()
        {
            await _action(_p1, _p2, _p3, _p4);
            TrySetResult(true);
        }

        private readonly Func<TParam1, TParam2, TParam3, TParam4, Task> _action;
        private readonly TParam1 _p1;
        private readonly TParam2 _p2;
        private readonly TParam3 _p3;
        private readonly TParam4 _p4;
    }
}
