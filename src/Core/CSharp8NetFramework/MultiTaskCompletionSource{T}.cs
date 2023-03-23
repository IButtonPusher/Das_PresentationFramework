using AssaultWare.Business;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace System.Threading.Tasks
{
   public class MultiTaskCompletionSource<T> : DelegateTaskCompletionSource<T>,
                                               IMultiTaskCompletionSource<T>
   {
      public MultiTaskCompletionSource(IMultiTaskCompletionSource<T> src)
      {
         Add(src, false);
      }

      public MultiTaskCompletionSource()
      {
         
      }

      public void Add(IMultiTaskCompletionSource<T> src,
                      Boolean ifSrcCompletionCompleteMe)
      {
         if (src.IsComplete)
            return;

         if (ifSrcCompletionCompleteMe)
            src.Task.ContinueWith(OnMergedCompleted);

         lock (_mergeLock)
         {
            _mergedSources ??= new List<IMultiTaskCompletionSource<T>>();
            _mergedSources.Add(src);
         }
      }

      private void OnMergedCompleted(Task<T> task)
      {
         TrySetResult(task.Result);
      }

      protected override void OnCompleted(Task<T> obj)
      {
         base.OnCompleted(obj);

         lock (_mergeLock)
         {
            if (_mergedSources is { } srcs)
            {
               foreach (var src in srcs)
               {
                  src.TrySetResult(obj.Result);
               }
            }
         }
      }

      private readonly Object _mergeLock = new();
      private List<IMultiTaskCompletionSource<T>>? _mergedSources = new();
   }
}
