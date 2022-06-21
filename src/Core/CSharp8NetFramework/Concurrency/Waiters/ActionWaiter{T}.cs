using System;
using System.Threading.Tasks;

namespace System.Threading
{
    public class ActionWaiter<TParam1> : DelegateWaiterBase
    {
        public ActionWaiter(TParam1 p1,
                               Action<TParam1> action, 
                               WorkerTypes workerType)
            : base(workerType, action)
        {
            _p1 = p1;
            _action = action;
        }

        public override void Execute()
        {
            _action(_p1);
            SetResult(true);
        }


        private readonly Action<TParam1> _action;
        private readonly TParam1 _p1;
        
    }
}
