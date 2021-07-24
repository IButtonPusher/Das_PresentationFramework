using System;
using System.Threading.Tasks;

#if !NET40
using TaskEx = System.Threading.Tasks.Task;
#endif

namespace System.Threading
{
    public abstract class FuncWaiterBase<TResult> : AsyncTaskCompletionSource<TResult>,
                                                     ITaskWaiter<TResult>
    {
        private readonly CancellationToken _cancellationToken;

        protected FuncWaiterBase(WorkerTypes workerType,
                                  Delegate forValidation,
                                  CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            DelegateWaiterBase.EnsureDelegateIsNotAsync(forValidation);
            WorkerType = workerType;
        }

        public virtual TaskStatus Status => Task.Status;

        public virtual WorkerTypes WorkerType { get; }

        public virtual void Cancel()
        {
            SetCanceled();
            TrySetResult(default!);
        }

        public virtual void BeginExecute()
        {
            try
            {
                var running = TaskEx.Run(Execute, _cancellationToken);
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

        public static implicit operator Task<TResult>(FuncWaiterBase<TResult> worker)
        {
            return worker.Task;
        }


        public virtual Task<TResult> GetResultAsync() => Task;


        public abstract TResult Execute();
    }
}
