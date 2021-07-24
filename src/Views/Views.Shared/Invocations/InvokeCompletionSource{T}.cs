using System;
using System.Threading.Tasks;

namespace Das.Views.Invocations
{
   public class InvokeCompletionSource<T> : TaskCompletionSource<T>
   {
      public InvokeCompletionSource(Func<Task<T>> action,
                                    Action<Action> invoke)
      {
         invoke(() =>
         {
            action()
               .ContinueWith(r =>
               {
                  var res = r.Result;
                  SetResult(res);
               });
         });
      }

      public InvokeCompletionSource(Func<T> action,
                                    Action<Action> invoke)
      {
         invoke(() =>
         {
            var res = action();
            SetResult(res);
         });
      }
   }
}
