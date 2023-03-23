using System;
using System.Threading;
using System.Threading.Tasks;

namespace AssaultWare.Business;

public class DelegateTaskCompletionSource<T> : AsyncTaskCompletionSource<T>
{
   private Int64 _completionCount;

   public DelegateTaskCompletionSource()
   {
      Task.ContinueWith(OnCompleted);
   }

   public DelegateTaskCompletionSource(CancellationToken cancellationToken)
      : this()
   {
      cancellationToken.Register(OnCancelled);
   }

   public T SetOrGetResult(Func<T> builder)
   {
      if (Interlocked.Add(ref _completionCount, 1) == 1)
      {
         var res = builder();
         if (TrySetResult(res))
            return res;
      }

      return Task.Result;
   }

   private void OnCancelled()
   {
      TrySetResult(default);
   }

   protected virtual void OnCompleted(Task<T> obj)
   {
      Interlocked.Add(ref _completionCount, 1);
   }

   public Boolean TrySetResult(Func<T>? builder)
   {
      if (Interlocked.Add(ref _completionCount, 1) == 1)
      {
         if (builder == null)
            return TrySetResult(default(T)!);

         return TrySetResult(builder());
      }

      return false;
   }

   
   //public T SetOrGetResult(Func<T> builder)
   //{
   //   if (Interlocked.Add(ref _completionCount, 1) == 1)
   //   {
   //      var res = builder();
   //      if (TrySetResult(res))
   //         return res;
   //   }

   //   return Task.Result;
   //}

   public async Task<Boolean> TrySetResultAsync(Func<Task<T>> builder)
   {
      if (Interlocked.Add(ref _completionCount, 1) == 1)
      {
         return TrySetResult(await builder());
      }

      return false;
   }
}
