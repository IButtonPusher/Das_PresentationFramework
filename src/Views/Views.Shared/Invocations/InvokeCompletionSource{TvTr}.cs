using System;
using System.Threading.Tasks;

namespace Das.Views.Invocations
{
   public class InvokeCompletionSource<TVisual, TResult> : TaskCompletionSource<TResult>
   {
      public InvokeCompletionSource(Func<TResult> action,
                                    TVisual visual,
                                    Action<TVisual, Action> nativeInvoke)
      {
         nativeInvoke(visual, () =>
         {
            var res = action();
            SetResult(res);
         });
      }
   }
}
