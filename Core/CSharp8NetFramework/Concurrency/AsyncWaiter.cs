using System;
using System.Threading.Tasks;

namespace System.Threading
{
    public class AsyncWaiter<TParam1, TParam2, TParam3, TResult> : AsyncWaiter<TResult>
    {
        public AsyncWaiter(TParam1 p1, 
                           TParam2 p2,
                           TParam3 p3,
                           Func<TParam1, TParam2, TParam3, TResult> action, 
                           WorkerTypes workerType)
            : base(workerType)
        {
            _p1 = p1;
            _p2 = p2;
            _p3 = p3;
            _action = action;
        }

        public override void Execute()
        {
            var res = _action(_p1, _p2, _p3);
            _completion.TrySetResult(res);
        }

        public static implicit operator Task<TResult>(AsyncWaiter<TParam1, TParam2, TParam3, TResult> worker)
        {
            return worker._completion.Task;
        }

        private readonly Func<TParam1, TParam2, TParam3, TResult> _action;
        private readonly TParam1 _p1;
        private readonly TParam2 _p2;
        private readonly TParam3 _p3;
    }

    public class AsyncWaiter<TParam1, TParam2, TResult> : AsyncWaiter<TResult>
    {
        public AsyncWaiter(TParam1 p1, TParam2 p2,
                           Func<TParam1, TParam2, TResult> action, WorkerTypes workerType)
            : base(workerType)
        {
            _p1 = p1;
            _p2 = p2;
            _action = action;
        }

        public override void Execute()
        {
            var res = _action(_p1, _p2);
            _completion.TrySetResult(res);
        }

        public static implicit operator Task<TResult>(AsyncWaiter<TParam1, TParam2, TResult> worker)
        {
            return worker._completion.Task;
        }

        private readonly Func<TParam1, TParam2, TResult> _action;
        private readonly TParam1 _p1;
        private readonly TParam2 _p2;
    }

    public class AsyncAwaiter<TParam, TResult> : AsyncWaiter<TResult>
    {
        public AsyncAwaiter(TParam p1, Func<TParam, Task<TResult>> action, WorkerTypes workerType)
            : base(workerType)
        {
            _p1 = p1;
            _action = action;
        }

        public override async void Execute()
        {
            try
            {
                var res = await _action(_p1);
                _completion.TrySetResult(res);
            }
            catch (Exception ex)
            {
                _completion.SetException(ex);
            }
        }

        public static implicit operator Task<TResult>(AsyncAwaiter<TParam, TResult> worker)
        {
            return worker._completion.Task;
        }

        private readonly Func<TParam, Task<TResult>> _action;
        private readonly TParam _p1;
    }

    public class AsyncWaiter<TParam, TResult> : AsyncWaiter<TResult>
    {
        public AsyncWaiter(TParam p1, Func<TParam, TResult> action, WorkerTypes workerType)
            : base(workerType)
        {
            _p1 = p1;
            _action = action;
        }

        public override void Execute()
        {
            var res = _action(_p1);
            _completion.TrySetResult(res);
        }

        public static implicit operator Task<TResult>(AsyncWaiter<TParam, TResult> worker)
        {
            return worker._completion.Task;
        }

        private readonly Func<TParam, TResult> _action;
        private readonly TParam _p1;
    }

    public class AsyncActionWaiter<TParam1, TParam2> : AsyncWaiter
    {
        public AsyncActionWaiter(TParam1 p1, 
                                 TParam2 p2,
                                 Action<TParam1, TParam2> action, WorkerTypes workerType)
            : base(workerType)
        {
            _p1 = p1;
            _p2 = p2;
            _action = action;
            _completion = new TaskCompletionSource<Boolean>();
        }

        public override void Execute()
        {
            _action(_p1, _p2);
            _completion.TrySetResult(true);
        }

        public override TaskStatus Status => _completion.Task?.Status ?? TaskStatus.Canceled;

        public override void Cancel()
        {
            _completion.TrySetResult(false);
        }


        public static implicit operator Task(AsyncActionWaiter<TParam1, TParam2> worker)
        {
            return worker._completion.Task;
        }

        private readonly Action<TParam1, TParam2> _action;
        private readonly TParam1 _p1;
        private readonly TParam2 _p2;
        protected readonly TaskCompletionSource<Boolean> _completion;
    }

    public class AsyncWaiter<TResult> : AsyncWaiter
    {
        public AsyncWaiter(Func<TResult> action,
                           WorkerTypes workerType)
            : base(workerType)
        {
            _action = action;
            _completion = new TaskCompletionSource<TResult>();
        }

        protected AsyncWaiter(WorkerTypes workerType) : base(workerType)
        {
            _action = () => throw new InvalidOperationException();
            _completion = new TaskCompletionSource<TResult>();
        }

        public override TaskStatus Status => _completion.Task?.Status ?? TaskStatus.Canceled;

        public override void Cancel()
        {
            _completion.TrySetResult(default!);
        }

        public override void Execute()
        {
            var res = _action();
            _completion.TrySetResult(res);
        }

        public static implicit operator Task<TResult>(AsyncWaiter<TResult> worker)
        {
            return worker._completion.Task;
        }


        private readonly Func<TResult> _action;
        protected readonly TaskCompletionSource<TResult> _completion;
    }

    public abstract class AsyncWaiter : ITaskWaiter
    {
        protected AsyncWaiter(WorkerTypes workerType)
        {
            WorkerType = workerType;
        }

        public WorkerTypes WorkerType { get; }

        public abstract TaskStatus Status { get; }

        public abstract void Execute();

        public abstract void Cancel();

        public override String ToString()
        {
            return $"{GetType()} {WorkerType}";
        }
    }

    public interface ITaskWaiter
    {
        TaskStatus Status { get; }

        WorkerTypes WorkerType { get; }

        void Cancel();

        void Execute();
    }

    public class SyncWaiter : ITaskWaiter
    {
        public SyncWaiter(WorkerTypes workerType)
        {
            WorkerType = workerType;
            Status = TaskStatus.WaitingForActivation;
        }

        public WorkerTypes WorkerType { get; }

        public TaskStatus Status { get; private set; }

        public void Execute()
        {
            Status = TaskStatus.WaitingToRun;
        }

        public void Cancel()
        {
            Status = TaskStatus.Canceled;
        }
    }


    public enum WorkerTypes
    {
        Reader,
        IteratorReader,

        Writer
    }
}