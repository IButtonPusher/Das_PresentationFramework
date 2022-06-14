using System;
using System.Threading.Tasks;

namespace System.Threading.Tasks
{
   public interface ITaskCompletionSource
   {
      Task Task { get; }
   }


   public interface ITaskCompletionSource<T> : ITaskCompletionSource
   {
      Boolean TrySetResult(T result);

      Boolean IsComplete { get; }

      new Task<T> Task { get; }
   }

   public interface IMultiTaskCompletionSource<T> : ITaskCompletionSource<T>
   {
      void Add(IMultiTaskCompletionSource<T> src,
               Boolean ifSrcCompletionCompleteMe);
   }
}
