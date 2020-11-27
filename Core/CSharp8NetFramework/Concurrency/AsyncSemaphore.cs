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

            toRelease.SetResult(true);
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

                var waiter = new TaskCompletionSource<Boolean>();
                _waiters.Enqueue(waiter);
                return waiter.Task;
            }
        }

        public void Wait()
        {
            TaskEx.Run(async () => await WaitAsync().ConfigureAwait(false)).Wait();
        }

        private readonly Task _completedTask = TaskEx.FromResult(true);

        private readonly Int32 _initialCount;
        private readonly Object _lockObj;
        private readonly Queue<TaskCompletionSource<Boolean>> _waiters;
        private Int32 _availableCount;
    }
}