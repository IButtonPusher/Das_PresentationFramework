using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace System.Threading
{
    public abstract class AsyncVoidTaskWaiterBase : AsyncTaskCompletionSource<Boolean>,
                                           IAsyncVoidTaskWaiter
    {
        public AsyncVoidTaskWaiterBase(WorkerTypes workerType,
                                       Delegate builder)
        {
            EnsureDelegateIsAsync<Task>(builder);

            WorkerType = workerType;
        }

        public TaskStatus Status => Task.Status;

        public WorkerTypes WorkerType { get; }

        public virtual void Cancel()
        {
            if (!Task.IsCompleted)
                SetCanceled();
        }

        public virtual void BeginExecute()
        {
            var ranning = ExecuteAsync();
            ranning.ConfigureAwait(false);
        }

        public virtual void ExecuteBlocking()
        {
            var ranning = ExecuteAsync();
            ranning.ConfigureAwait(false).GetAwaiter().GetResult();
        }


        public abstract Task ExecuteAsync();
        


        public static implicit operator Task(AsyncVoidTaskWaiterBase worker)
        {
            return worker.Task;
        }

        public static void EnsureDelegateIsAsync<TTask>(Delegate forValidation)
            where TTask : Task
        {
            if (!(forValidation is Func<TTask>))
                throw new InvalidAsynchronousStateException("delegate " + forValidation + " is not asynchronous.  " +
                                                            "Use AsyncWaiterBase derived types for this.");
        }
    }
}
