using System;
using System.Threading.Tasks;

namespace System.Threading
{
    public class FuncWaiter<TResult> : FuncWaiterBase<TResult>
    {
        public FuncWaiter(Func<TResult> action,
                          WorkerTypes workerType,
                          CancellationToken cancellationToken)
            : base(workerType, action, cancellationToken)
        {
            _action = action;
        }

        public override TResult Execute()
        {
            var res = _action();
            SetResult(res);
            return res;
        }

        public static implicit operator Task<TResult>(FuncWaiter<TResult> worker)
        {
            return worker.Task;
        }


        private readonly Func<TResult> _action;
    }


 

}
