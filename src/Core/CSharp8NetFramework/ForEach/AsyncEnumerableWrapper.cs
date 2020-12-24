using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncResults.Enumerable
{
    internal sealed class AsyncEnumerableWrapper<T> : IAsyncEnumerable, IAsyncEnumerable<T>
    {
        public AsyncEnumerableWrapper(IEnumerable<T> enumerable, Boolean runSynchronously)
        {
            _enumerable = enumerable;
            _runSynchronously = runSynchronously;
        }

        IAsyncEnumerator IAsyncEnumerable.GetAsyncEnumerator(CancellationToken cancellationToken)
        {
            return new AsyncEnumeratorWrapper<T>(_enumerable.GetEnumerator(), _runSynchronously)
                {MasterCancellationToken = cancellationToken};
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new AsyncEnumeratorWrapper<T>(_enumerable.GetEnumerator(), _runSynchronously)
                {MasterCancellationToken = cancellationToken};
        }

        private readonly IEnumerable<T> _enumerable;
        private readonly Boolean _runSynchronously;
    }
}