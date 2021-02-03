using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#if !NET40
using TaskEx = System.Threading.Tasks.Task;

#endif


namespace System.Threading
{
    public class AsyncSemaphore
    {
        public AsyncSemaphore(Int32 initialCount)
        {
            _initialCount = initialCount;
            _lockObj = new Object();
            _waiters = new Queue<TaskCompletionSource<Boolean>>();


            if (initialCount < 0)
                throw new ArgumentOutOfRangeException(nameof(initialCount));
            _availableCount = initialCount;
        }

        public Int32 CurrentCount => _initialCount - _availableCount;

        public void Release()
        {
            TaskCompletionSource<Boolean> toRelease;
            lock (_lockObj)
            {
                if (_waiters.Count > 0)
                    toRelease = _waiters.Dequeue();
                else
                {
                    ++_availableCount;
                    return;
                }
            }

            toRelease.TrySetResult(true);
        }

        public Task WaitAsync(CancellationToken cancellationToken)
        {
            lock (_lockObj)
            {
                if (_availableCount > 0)
                {
                    --_availableCount;
                    return _completedTask;
                }

                var waiter = new AsyncTaskCompletionSource<Boolean>();
                _waiters.Enqueue(waiter);

                cancellationToken.Register(
                    OnCancelled, waiter);

                return waiter.Task;
            }
        }

        private static void OnCancelled(Object state)
        {
            if (!(state is TaskCompletionSource<Boolean> valid))
                return;

            if (valid.Task.Status == TaskStatus.Faulted ||
                valid.Task.Status == TaskStatus.RanToCompletion ||
                valid.Task.Status == TaskStatus.Canceled)
                return;

            valid.TrySetCanceled();
        }

        public Task WaitAsync()
        {
            lock (_lockObj)
            {
                if (_availableCount > 0)
                {
                    --_availableCount;
                    return _completedTask;
                }

                var waiter = new AsyncTaskCompletionSource<Boolean>();
                _waiters.Enqueue(waiter);
                return waiter.Task;
            }
        }

        public void Wait()
        { 
            lock (_lockObj)
            {
                if (_availableCount > 0)
                {
                    --_availableCount;
                    return;
                }
            }

            var waiter = new TaskCompletionSource<Boolean>();
            _waiters.Enqueue(waiter);

            var waited = 2;

            //waiter.Task.ContinueWith(OnSynchronousWaiterFinished, new Object());

            while (waiter.Task.Status == TaskStatus.Running)
            {
                Thread.Sleep(waited);

                waited = Math.Min(MAX_SYNCH_THREAD_SLEEP, waited * waited);
            }


            //TaskEx.Run(async () => await WaitAsync().ConfigureAwait(false)).Wait();
        }


        private const Int32 MAX_SYNCH_THREAD_SLEEP = 256;

        private readonly Task _completedTask = TaskEx.FromResult(true);
        private readonly Int32 _initialCount;
        private readonly Object _lockObj;
        private readonly Queue<TaskCompletionSource<Boolean>> _waiters;
        private Int32 _availableCount;
    }
}