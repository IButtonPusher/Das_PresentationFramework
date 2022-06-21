using System;
using System.Threading.Tasks;

namespace System.Threading
{
    public class ActionWaiter<TParam1, TParam2> : DelegateWaiterBase
    {
        public ActionWaiter(TParam1 p1, 
                               TParam2 p2,
                               Action<TParam1, TParam2> action, 
                               WorkerTypes workerType)
            : base(workerType, action)
        {
            _p1 = p1;
            _p2 = p2;
            _action = action;
        }


        public override void Execute()
        {
            _action(_p1, _p2);
            SetResult(true);
        }


        private readonly Action<TParam1, TParam2> _action;
        private readonly TParam1 _p1;
        private readonly TParam2 _p2;
    }
}
