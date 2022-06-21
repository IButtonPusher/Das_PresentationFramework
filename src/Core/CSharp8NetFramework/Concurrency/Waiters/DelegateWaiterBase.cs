using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

#if !NET40
using TaskEx = System.Threading.Tasks.Task;
#endif

namespace System.Threading
{
    public abstract class DelegateWaiterBase : AsyncTaskCompletionSource<Boolean>,
                                               ITaskWaiter
    {
        protected DelegateWaiterBase(WorkerTypes workerType,
                                     Delegate forValidation)
        {
            EnsureDelegateIsNotAsync(forValidation);

            WorkerType = workerType;
        }

        public WorkerTypes WorkerType { get; }

        public virtual TaskStatus Status => Task.Status;


        public virtual void Cancel()
        {
            SetCanceled();
            TrySetResult(false);
        }

        public virtual void BeginExecute()
        {
            try
            {
                var running = TaskEx.Run(Execute);
                running.ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                SetCanceled();
            }
        }

        public virtual void ExecuteBlocking()
        {
            Execute();
        }

        public override String ToString()
        {
            return $"{GetType()} {WorkerType}";
        }

        public static implicit operator Task(DelegateWaiterBase worker)
        {
            return worker.Task;
        }

        [Conditional("DEBUG")]
        public static void EnsureDelegateIsNotAsync(Delegate forValidation)
        {
            if (forValidation is Func<Task> bad)
                throw new InvalidAsynchronousStateException("delegate " + bad + " is asynchronous.  " +
                                                            "Use AsyncTaskWaiterBase derived types for this.");
        }


        public abstract void Execute();

    }
}
