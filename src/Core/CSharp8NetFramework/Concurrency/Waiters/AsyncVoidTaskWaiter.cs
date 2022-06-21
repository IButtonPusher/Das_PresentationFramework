using System;
using System.Threading.Tasks;

namespace System.Threading
{
   public class AsyncVoidTaskWaiter : AsyncVoidTaskWaiterBase
   {
      public AsyncVoidTaskWaiter(Func<Task> action,
                                 WorkerTypes workerType)
         : base(workerType, action)
      {
         _action = action;
      }

      public override async Task ExecuteAsync()
      {
         await _action();
         TrySetResult(true);
      }

      private readonly Func<Task> _action;
   }
}
