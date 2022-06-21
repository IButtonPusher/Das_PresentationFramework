using System;
using System.Threading.Tasks;

namespace System.Threading
{
    public class FuncWaiter<TParam, TResult> : FuncWaiterBase<TResult>
    {
        public FuncWaiter(TParam p1, 
                           Func<TParam, TResult> action, 
                           WorkerTypes workerType,
                           CancellationToken cancellationToken)
            : base(workerType, action, cancellationToken)
        {
            _p1 = p1;
            _action = action;
        }

        public override TResult Execute()
        {
            var res = _action(_p1);
            SetResult(res);
            return res;
        }

        public static implicit operator Task<TResult>(FuncWaiter<TParam, TResult> worker)
        {
            return worker.Task;
        }

        private readonly Func<TParam, TResult> _action;
        private readonly TParam _p1;
    }
}
