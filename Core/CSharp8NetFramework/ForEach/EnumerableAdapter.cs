using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

// ReSharper disable All

namespace AsyncResults.Enumerable
{
    #if !NETSTANDARD2_1 && !NETSTANDARD2_0 && !NET461
    internal sealed class EnumerableAdapter : IEnumerable
    {
        public EnumerableAdapter(IAsyncEnumerable asyncEnumerable)
        {
            _asyncEnumerable = asyncEnumerable;
        }

        public IEnumerator GetEnumerator() =>
            new EnumeratorAdapter(_asyncEnumerable.GetAsyncEnumerator());

        private readonly IAsyncEnumerable _asyncEnumerable;
    }
    #endif

    internal sealed class EnumerableAdapter<T> : IEnumerable, IEnumerable<T>
    {
        public EnumerableAdapter(IAsyncEnumerable<T> asyncEnumerable)
        {
            _asyncEnumerable = asyncEnumerable;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<T> GetEnumerator() =>
            new EnumeratorAdapter<T>(_asyncEnumerable.GetAsyncEnumerator());

        private readonly IAsyncEnumerable<T> _asyncEnumerable;
    }
}