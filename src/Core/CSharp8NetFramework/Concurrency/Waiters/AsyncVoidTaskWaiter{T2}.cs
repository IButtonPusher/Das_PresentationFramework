using System;
using System.Threading.Tasks;

namespace System.Threading
{
    public class AsyncVoidTaskWaiter<TParam1, TParam2> : AsyncVoidTaskWaiterBase
    {
        private readonly Func<TParam1, TParam2, Task> _action;
        private readonly TParam1 _p1;
        private readonly TParam2 _p2;

        public AsyncVoidTaskWaiter(TParam1 p1, 
                                   TParam2 p2,
                                   Func<TParam1, TParam2, Task> action,
                                   WorkerTypes workerType) 
            : base(workerType, action)
        {
            _p1 = p1;
            _p2 = p2;
            _action = action;
        }

        public override async Task ExecuteAsync()
        {
            await _action(_p1, _p2);
            TrySetResult(true);
        }
    }
}
