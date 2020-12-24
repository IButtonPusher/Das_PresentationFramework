using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#pragma warning disable 8603

namespace AsyncResults.Enumerable
{
    internal sealed class EmptyAsyncEnumerator<T> : IAsyncEnumerator, IAsyncEnumerator<T>
    {
        Object IAsyncEnumerator.Current => Current;

        public ValueTask<Boolean> MoveNextAsync()
        {
            return new ValueTask<Boolean>(false);
        }

        public ValueTask DisposeAsync()
        {
            return new ValueTask();
        }

        public void Dispose()
        {
        }

        public T Current => throw new InvalidOperationException("The enumerator has reached the end of the collection");
    }
}