using System;
using System.Threading.Tasks;

namespace System.Threading
{
    public class FuncWaiter<TParam1, TParam2, TParam3, TResult> : FuncWaiterBase<TResult>
    {
        public FuncWaiter(TParam1 p1, 
                           TParam2 p2,
                           TParam3 p3,
                           Func<TParam1, TParam2, TParam3, TResult> action, 
                           WorkerTypes workerType,
                           CancellationToken cancellationToken)
            : base(workerType, action, cancellationToken)
        {
            _p1 = p1;
            _p2 = p2;
            _p3 = p3;
            _action = action;
        }

        public override void Cancel()
        {
        }

        public override TResult Execute()
        {
            var res = _action(_p1, _p2, _p3);
            SetResult(res);

            return res;
        }

        public static implicit operator Task<TResult>(FuncWaiter<TParam1, TParam2, TParam3, TResult> worker)
        {
            return worker.Task;
        }

        private readonly Func<TParam1, TParam2, TParam3, TResult> _action;
        private readonly TParam1 _p1;
        private readonly TParam2 _p2;
        private readonly TParam3 _p3;
    }
}
