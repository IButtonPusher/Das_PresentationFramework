using System;
using System.Threading.Tasks;
using TaskEx = System.Threading.Tasks.Task;

namespace System.Threading
{
    public readonly struct WaiterAction
    {
        private readonly ITaskWaiter _waiter;
        private readonly Action<ITaskWaiter> _execute;

        public WaiterAction(ITaskWaiter waiter,
                            Action<ITaskWaiter> execute)
        {
            _waiter = waiter;
            _execute = execute;
        }

        public void Execute()
        {
            _execute(_waiter);
        }
    }
}