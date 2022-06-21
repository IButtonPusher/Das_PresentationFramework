using System;

namespace System.Threading
{
    public class ActionWaiter<TParam1, TParam2, TParam3> : DelegateWaiterBase
    {
        public ActionWaiter(TParam1 p1, 
                               TParam2 p2,
                               TParam3 p3,
                               Action<TParam1, TParam2, TParam3> action, 
                               WorkerTypes workerType)
            : base(workerType, action)
        {
            _p1 = p1;
            _p2 = p2;
            _p3 = p3;
            _action = action;
            
        }

        public override void Execute()
        {
            _action(_p1, _p2, _p3);
            SetResult(true);
        }

        private readonly Action<TParam1, TParam2, TParam3> _action;
        private readonly TParam1 _p1;
        private readonly TParam2 _p2;
        private readonly TParam3 _p3;
    }

}
