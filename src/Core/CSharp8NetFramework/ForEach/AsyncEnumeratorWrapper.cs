using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#if !NET40

using TaskEx = System.Threading.Tasks.Task;

#pragma warning disable 8603
#endif


namespace AsyncResults.Enumerable
{
    internal sealed class AsyncEnumeratorWrapper<T> : IAsyncEnumerator, IAsyncEnumerator<T>
    {
        public AsyncEnumeratorWrapper(IEnumerator<T> enumerator, Boolean runSynchronously)
        {
            _enumerator = enumerator;
            _runSynchronously = runSynchronously;
        }

        Object IAsyncEnumerator.Current => Current!;

        public ValueTask<Boolean> MoveNextAsync()
        {
            if (!_runSynchronously)
                return new ValueTask<Boolean>(TaskEx.Run(() => _enumerator.MoveNext(), MasterCancellationToken));

            try
            {
                return new ValueTask<Boolean>(_enumerator.MoveNext());
            }
            catch (Exception ex)
            {
                var tcs = new TaskCompletionSource<Boolean>();
                tcs.SetException(ex);
                return new ValueTask<Boolean>(tcs.Task);
            }
        }

        public void Dispose()
        {
            _enumerator.Dispose();
        }

        public ValueTask DisposeAsync()
        {
            Dispose();
            return new ValueTask();
        }

        public T Current => _enumerator.Current;

        private readonly IEnumerator<T> _enumerator;
        private readonly Boolean _runSynchronously;

        internal CancellationToken MasterCancellationToken;
    }
}