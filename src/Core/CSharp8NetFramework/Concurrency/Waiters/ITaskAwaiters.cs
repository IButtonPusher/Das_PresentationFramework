using System;
using System.Threading.Tasks;

namespace System.Threading
{
    public interface ITaskWaiter
    {
        TaskStatus Status { get; }

        WorkerTypes WorkerType { get; }

        void Cancel();

        void BeginExecute();

        void ExecuteBlocking();
    }

    public interface IAsyncVoidTaskWaiter : ITaskWaiter
    {
        Task ExecuteAsync();
    }

    public interface ITaskWaiter<TResult> : ITaskWaiter
    {
        Task<TResult> GetResultAsync();

        TResult Execute();
    }


    public interface IAsyncTaskWaiter<TResult> : ITaskWaiter
    {
        Task<TResult> GetResultAsync();
    }
}
