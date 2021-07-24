using System;
using System.Threading.Tasks;

namespace System.Threading
{
    public class FuncWaiter<TParam1, TParam2, TResult> : FuncWaiterBase<TResult>
    {
        public FuncWaiter(TParam1 p1,
                           TParam2 p2,
                           Func<TParam1, TParam2, TResult> action,
                           WorkerTypes workerType,
                           CancellationToken cancellationToken)
            : base(workerType, action, cancellationToken)
        {
            _p1 = p1;
            _p2 = p2;
            _action = action;
        }

        public override TResult Execute()
        {
            var res = _action(_p1, _p2);
            TrySetResult(res);
            return res;
        }


        private readonly Func<TParam1, TParam2, TResult> _action;
        private readonly TParam1 _p1;
        private readonly TParam2 _p2;
    }
}
