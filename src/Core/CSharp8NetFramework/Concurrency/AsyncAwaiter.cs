using System;
using System.Threading.Tasks;


#if !NET40
using TaskEx = System.Threading.Tasks.Task;
#endif

namespace System.Threading
{
    public class AsyncAwaiter<TParam1, TParam2, TResult> : AsyncWaiter<TResult>
    {
        public AsyncAwaiter(TParam1 p1, 
                            TParam2 p2,
                            Func<TParam1, TParam2, Task<TResult>> action, 
                            WorkerTypes workerType)
            : base(workerType)
        {
            _p1 = p1;
            _p2 = p2;
            _action = action;
        }

        public override async void Execute()
        {
            try
            {
                var res = await _action(_p1, _p2);
                _completion.TrySetResult(res);
            }
            catch (Exception ex)
            {
                _completion.SetException(ex);
            }
        }

        public static implicit operator Task<TResult>( AsyncAwaiter<TParam1, TParam2, TResult> worker)
        {
            return worker._completion.Task;
        }

        private readonly Func<TParam1, TParam2, Task<TResult>> _action;
        private readonly TParam1 _p1;
        private readonly TParam2 _p2;
    }

    public class AsyncAwaiter<TParam1, TParam2> : AsyncWaiter<Task>
    {
        public AsyncAwaiter(TParam1 p1, 
                            TParam2 p2,
                            Func<TParam1, TParam2, Task> action, 
                            WorkerTypes workerType)
            : base(workerType)
        {
            _p1 = p1;
            _p2 = p2;
            _action = action;
        }

        public override async void Execute()
        {
            try
            {
                await _action(_p1, _p2);
                _completion.TrySetResult(TaskEx.CompletedTask);
            }
            catch (Exception ex)
            {
                _completion.SetException(ex);
            }
        }

        public static implicit operator Task(AsyncAwaiter<TParam1, TParam2> worker)
        {
            return worker._completion.Task;
        }

        private readonly Func<TParam1, TParam2, Task> _action;
        private readonly TParam1 _p1;
        private readonly TParam2 _p2;
    }

    public class AsyncTaskAwaiter<TParam, TResult> : AsyncWaiter<TResult>
    {
        public AsyncTaskAwaiter(TParam p1, Func<TParam, Task<TResult>> action, WorkerTypes workerType)
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

        public static implicit operator Task<TResult>(AsyncTaskAwaiter<TParam, TResult> worker)
        {
            return worker._completion.Task;
        }

        private readonly Func<TParam, Task<TResult>> _action;
        private readonly TParam _p1;
    }
}
