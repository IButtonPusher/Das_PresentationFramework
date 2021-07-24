using System;
using System.Threading.Tasks;

#if !NET40
using TaskEx = System.Threading.Tasks.Task;
#endif


namespace System.Threading
{
    public partial class ReaderWriterAsync
    {
        public Task<TResult> ReadAsync<TResult>(Func<TResult> action)
        {
            if (_isDisposed)
                return TaskEx.FromResult(default(TResult)!);


            if (TryGetReaderWaiter((r,
                                    c) => new FuncWaiter<TResult>(action, r, c),
                ref _readerCount, out var waiter))
                return waiter;

            try
            {
                var res = action();
                return TaskEx.FromResult(res);
            }
            finally
            {
                EndReaderImpl();
            }
        }


        public async Task<TResult> ReadTaskAsync<TParam, TResult>(TParam param,
                                                                  Func<TParam, Task<TResult>> action)
        {
            if (_isDisposed)
                return default!;

            AsyncTaskAwaiter<TParam, TResult>? waiter;

            lock (_lockObj)
            {
                if (_writerCount > 0 || _waiterCount > 0)
                {
                    _waiterCount++;
                    waiter = new AsyncTaskAwaiter<TParam, TResult>(param,
                        action, WorkerTypes.Reader);
                    _waiters.Enqueue(waiter);
                }
                else
                {
                    waiter = default;
                    _readerCount++;
                }
            }

            if (waiter != null)
            {
                //can't run immediately due to a writer and/or waiters
                Task<TResult> allSet = waiter;
                await allSet;
            }


            try
            {
                var res = await action(param);
                return res;
            }
            finally
            {
                EndReaderImpl();
            }
        }


        public Task<TResult> ReadAsync<TParam1, TResult>(TParam1 p1,
                                                         Func<TParam1, TResult> action)
        {
            if (_isDisposed)
                return TaskEx.FromResult(default(TResult)!);

            if (TryGetReaderWaiter((r,
                                    c) => new FuncWaiter<TParam1, TResult>(
                    p1, action, r, c),
                ref _readerCount,
                out var waiter))
                return waiter;

            try
            {
                var res = action(p1);
                return TaskEx.FromResult(res);
            }
            finally
            {
                EndReaderImpl();
            }
        }

        public Task<TResult> ReadAsync<TParam1, TParam2, TParam3, TResult>(TParam1 p1,
                                                                           TParam2 p2,
                                                                           TParam3 p3,
                                                                           Func<TParam1, TParam2, TParam3, TResult>
                                                                               action)
        {
            if (_isDisposed)
                return TaskEx.FromResult(default(TResult)!);


            if (TryGetReaderWaiter((r,
                                    c) => new FuncWaiter<TParam1, TParam2, TParam3, TResult>(
                    p1, p2, p3, action, r, c),
                ref _readerCount,
                out var waiter))
                return waiter;

            try
            {
                var res = action(p1, p2, p3);
                return TaskEx.FromResult(res);
            }
            finally
            {
                EndReaderImpl();
            }
        }

        public Task<TResult> ReadAsync<TParam1, TParam2, TResult>(TParam1 p1,
                                                                  TParam2 p2,
                                                                  Func<TParam1, TParam2, TResult> action)
        {
            if (_isDisposed)
                return TaskEx.FromResult(default(TResult)!);

            if (TryGetReaderWaiter((r,
                                    c) => new FuncWaiter<TParam1, TParam2, TResult>(
                    p1, p2, action, r, c),
                ref _readerCount,
                out var waiter))
                return waiter;

            try
            {
                var res = action(p1, p2);
                return TaskEx.FromResult(res);
            }
            finally
            {
                EndReaderImpl();
            }
        }


        /// <summary>
        ///     increments reader count if no waiter is needed!
        /// </summary>
        private Boolean TryGetReaderWaiter<TReaderWaiter>(Func<WorkerTypes, CancellationToken, TReaderWaiter> builder,
                                                          ref Int32 readerCount,
                                                          out TReaderWaiter value)
            where TReaderWaiter : ITaskWaiter
        {
            lock (_lockObj)
            {
                if (_writerCount > 0 || _waiterCount > 0)
                {
                    _waiterCount++;
                    value = builder(WorkerTypes.Reader, _disposeClearCancellation.Token);
                    _waiters.Enqueue(value);
                    return true;
                }

                readerCount++;

                value = default!;
                return false;
            }
        }
    }
}
