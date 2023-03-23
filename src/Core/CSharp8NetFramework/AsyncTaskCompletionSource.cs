using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace System.Threading.Tasks
{
   public class AsyncTaskCompletionSource<T> : TaskCompletionSource<T>,
                                               ITaskCompletionSource<T>
   {
      static AsyncTaskCompletionSource()
      {
         var strType = typeof(String);
         var assemblyUri = strType.Assembly.CodeBase;

         var fileName = new Uri(assemblyUri).LocalPath;
         var dothExist = File.Exists(fileName);

         if (!dothExist ||
             (FileVersionInfo.GetVersionInfo(fileName) is { } vi &&
              vi.FileMajorPart >= 4 &&
              vi.FileMinorPart >= 5))
         {
            _defaultCreationOptions = (TaskCreationOptions) 64;
         }
         else
         {
            _defaultCreationOptions = TaskCreationOptions.None;
         }
      }

      /// <summary>
      ///    Sets the result to default if cancelled, does not throw a TaskCanceledException
      /// </summary>
      public AsyncTaskCompletionSource(CancellationToken cancellationToken)
         : this()
      {
         cancellationToken.Register(OnCancelled);
      }

      public AsyncTaskCompletionSource() : base(_defaultCreationOptions)
      {
         //Interlocked.Add(ref _cnt, 1);
         //Task.ContinueWith(OnCompleted);
      }

      Task ITaskCompletionSource.Task => Task;

      public Boolean IsComplete => Task.IsCompleted;

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

      //public Boolean TrySetResult(Func<T>? builder)
      //{
      //   if (Interlocked.Add(ref _completionCount, 1) == 1)
      //   {
      //      if (builder == null)
      //         return TrySetResult(default(T)!);

      //      return TrySetResult(builder());
      //   }

      //   return false;
      //}

      //public async Task<Boolean> TrySetResultAsync(Func<Task<T>> builder)
      //{
      //   if (Interlocked.Add(ref _completionCount, 1) == 1)
      //   {
      //      return TrySetResult(await builder());
      //   }

      //   return false;
      //}

      private void OnCancelled()
      {
         TrySetResult(default!);
      }

      //protected virtual void OnCompleted(Task<T> obj)
      //{
      //   Interlocked.Add(ref _completionCount, 1);
      //}



      private static readonly TaskCreationOptions _defaultCreationOptions;

      //private static Int32 _cnt;


      //private Int64 _completionCount;
   }
}
