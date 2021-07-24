using System;
using System.Threading.Tasks;

namespace System.Threading
{
    public class AsyncTaskAwaiter<TParam, TResult> : AsyncTaskWaiterBase<TResult>
    {
        public AsyncTaskAwaiter(TParam p1,
                                Func<TParam, Task<TResult>> action,
                                WorkerTypes workerType)
            : base(workerType, action)
        {
            _p1 = p1;
            _action = action;
        }

        public override async Task<TResult> GetResultAsync()
        {
            var res = await _action(_p1);
            SetResult(res);

            return res;
        }

        private readonly Func<TParam, Task<TResult>> _action;
        private readonly TParam _p1;
    }
}
