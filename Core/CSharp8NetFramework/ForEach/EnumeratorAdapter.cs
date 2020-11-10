using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

// ReSharper disable All
#pragma warning disable 8603

namespace AsyncResults.Enumerable
{
    #if !NETSTANDARD2_1 && !NETSTANDARD2_0 && !NET461
    internal sealed class EnumeratorAdapter : IEnumerator
    {
        public EnumeratorAdapter(IAsyncEnumerator asyncEnumerator)
        {
            _asyncEnumerator = asyncEnumerator;
        }

        public object Current => _asyncEnumerator.Current;

        public bool MoveNext() => _asyncEnumerator.MoveNextAsync().GetAwaiter().GetResult();

        public void Reset() =>
            throw new NotSupportedException(
                "The IEnumerator.Reset() method is obsolete. Create a new enumerator instead.");

        public void Dispose() => _asyncEnumerator.Dispose();

        private readonly IAsyncEnumerator _asyncEnumerator;
    }
    #endif

    internal sealed class EnumeratorAdapter<T> : IEnumerator, IEnumerator<T>
    {
        public EnumeratorAdapter(IAsyncEnumerator<T> asyncEnumerator)
        {
            _asyncEnumerator = asyncEnumerator;
        }

        Object IEnumerator.Current => Current;

        public Boolean MoveNext() => _asyncEnumerator.MoveNextAsync().GetAwaiter().GetResult();

        public void Reset() =>
            throw new NotSupportedException(
                "The IEnumerator.Reset() method is obsolete. Create a new enumerator instead.");

        public T Current => _asyncEnumerator.Current;

        public void Dispose() =>
            _asyncEnumerator.DisposeAsync().GetAwaiter().GetResult();

        private readonly IAsyncEnumerator<T> _asyncEnumerator;
    }
}