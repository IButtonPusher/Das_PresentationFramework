using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace System.Threading
{
    public abstract class AsyncTaskWaiterBase<TResult> : AsyncTaskCompletionSource<TResult>,
                                                       IAsyncTaskWaiter<TResult>
    {
        public AsyncTaskWaiterBase(WorkerTypes workerType,
                                   Delegate builder)
        {
            EnsureDelegateIsAsync<Task<TResult>>(builder);

            WorkerType = workerType;
        }

        public abstract Task<TResult> GetResultAsync();

        public TaskStatus Status => Task.Status;

        public WorkerTypes WorkerType { get; }

        public void Cancel()
        {
        }

        public virtual void BeginExecute()
        {
            var ranning = GetResultAsync();
            ranning.ConfigureAwait(false);
        }

        public virtual void ExecuteBlocking()
        {
            var ranning = GetResultAsync();
            ranning.ConfigureAwait(false).GetAwaiter().GetResult();
        }


        public static void EnsureDelegateIsAsync<TTask>(Delegate forValidation)
        where TTask : Task
        {
            if (!(forValidation is Func<TTask>))
                throw new InvalidAsynchronousStateException("delegate " + forValidation + " is not asynchronous.  " +
                                                            "Use AsyncWaiterBase derived types for this.");
        }

        public static implicit operator Task<TResult>(AsyncTaskWaiterBase<TResult> worker)
        {
            return worker.Task;
        }


       
    }
    
}
