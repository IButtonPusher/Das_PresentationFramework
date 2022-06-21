using System;
using System.Threading.Tasks;

namespace System.Threading
{
    public class AsyncTaskAwaiter<TParam1, TParam2, TResult> : AsyncTaskWaiterBase<TResult>
    {
        public AsyncTaskAwaiter(TParam1 p1, 
                            TParam2 p2,
                            Func<TParam1, TParam2, Task<TResult>> action, 
                            WorkerTypes workerType)
            : base(workerType, action)
        {
            _p1 = p1;
            _p2 = p2;
            _action = action;
        }


        private readonly Func<TParam1, TParam2, Task<TResult>> _action;
        private readonly TParam1 _p1;
        private readonly TParam2 _p2;

        public override async Task<TResult> GetResultAsync()
        {
            var res = await _action(_p1, _p2);
            TrySetResult(res);

            return res;
        }
    }
}
