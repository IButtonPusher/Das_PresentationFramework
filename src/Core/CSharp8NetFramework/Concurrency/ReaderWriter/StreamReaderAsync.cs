using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#if !NET40
using TaskEx = System.Threading.Tasks.Task;
#endif

namespace System.Threading
{
    public partial class ReaderWriterAsync
    {
        public async IAsyncEnumerable<TResult> ReadManyAsync<TResult>(Func<IEnumerable<TResult>> action)
        {
            var dothContinue = await WaitForIteratorAsync();

            if (!dothContinue)
                yield break;

            try
            {
                foreach (var a in action())
                {
                    if (_isDisposed)
                        yield break;
                    yield return a;
                }
            }
            finally
            {
                EndReaderImpl();
            }
        }

        public async IAsyncEnumerable<TResult> ReadManyAsync<TParam1, TResult>(
            Func<TParam1, IEnumerable<TResult>> action,
            TParam1 p1)
        {
            var dothContinue = await WaitForIteratorAsync();

            if (!dothContinue)
                yield break;


            try
            {
                foreach (var a in action(p1))
                {
                    if (_isDisposed)
                        yield break;
                    yield return a;
                }
            }
            finally
            {
                EndReaderImpl();
            }
        }

        public async IAsyncEnumerable<TResult> ReadManyAsync<TParam1, TParam2, TParam3, TResult>(
            Func<TParam1, TParam2, TParam3, IEnumerable<TResult>> action,
            TParam1 p1,
            TParam2 p2,
            TParam3 p3)
        {
            var dothContinue = await WaitForIteratorAsync();

            if (!dothContinue)
                yield break;

            try
            {
                foreach (var a in action(p1, p2, p3))
                {
                    if (_isDisposed)
                        yield break;
                    yield return a;
                }
            }
            finally
            {
                EndReaderImpl();
            }
        }

        public async IAsyncEnumerable<TResult> ReadManyAsync<TParam, TResult>(TParam p1,
                                                                              Func<TParam, IAsyncEnumerable<TResult>>
                                                                                  action)
        {
            var dothContinue = await WaitForIteratorAsync();

            if (!dothContinue)
                yield break;

            try
            {
                await foreach (var a in action(p1))
                {
                    if (_isDisposed)
                        yield break;
                    yield return a;
                }
            }
            finally
            {
               EndReaderImpl();
            }
        }

        public async IAsyncEnumerable<TResult> ReadManyAsync<TParam1, TParam2, TResult>(
            Func<TParam1, TParam2, IEnumerable<TResult>> action,
            TParam1 p1,
            TParam2 p2)
        {
            var dothContinue = await WaitForIteratorAsync().ConfigureAwait(false);

            if (!dothContinue)
                yield break;

            try
            {
                foreach (var a in action(p1, p2))
                {
                    if (_isDisposed)
                        yield break;
                    yield return a;
                }
            }
            finally
            {
                EndReaderImpl();
            }
        }


        private Task<Boolean> WaitForIteratorAsync()
        {
            if (_isDisposed)
                return TaskEx.FromResult(false);

            Task<Boolean> waitFor;

            lock (_lockObj)
            {
                if (_writerCount > 0 || _waiterCount > 0)
                {
                    _waiterCount++;
                    var waiter = new FuncWaiter<Boolean>(() => true, WorkerTypes.IteratorReader,
                        _disposeClearCancellation.Token);
                    _waiters.Enqueue(waiter);
                    waitFor = waiter;
                }
                else
                {
                    waitFor = TaskEx.FromResult(true);
                    _readerCount++;
                }
            }

            return waitFor;
        }
    }
}
