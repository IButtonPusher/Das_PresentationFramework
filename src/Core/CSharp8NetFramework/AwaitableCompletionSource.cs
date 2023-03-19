using System;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices;

//public class AwaitableCompletionSource<TResult, TStateMachine> : AsyncTaskCompletionSource<TResult>
//   where TStateMachine : IAsyncStateMachine
//{
//   private readonly TStateMachine _stateMachine;

//   public AwaitableCompletionSource(TStateMachine stateMachine)
//   {
//      _stateMachine = stateMachine;
//   }
//}

public class AwaitableCompletionSource<TResult> : AsyncTaskCompletionSource<TResult>
{
   public AwaitableCompletionSource(IAsyncStateMachine stateMachine)
   {
      _stateMachine = stateMachine;
   }

   public new Boolean TrySetException(Exception ex)
   {
      var wex = new StateAggregateException(ex, _stateMachine);

      return base.TrySetException(wex);
   }

   private readonly IAsyncStateMachine _stateMachine;
}
