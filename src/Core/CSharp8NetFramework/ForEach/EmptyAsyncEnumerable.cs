using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AsyncResults.Enumerable;

namespace AsyncResults.ForEach
{
    internal sealed class EmptyAsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return AsyncEnumerator<T>.Empty;
        }
    }
}