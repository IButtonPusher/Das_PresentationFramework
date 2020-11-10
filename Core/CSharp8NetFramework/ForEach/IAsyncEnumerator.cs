using System;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    public interface IAsyncEnumerator : IDisposable, IAsyncDisposable
    {
        /// <summary>Gets the current element in the collection.</summary>
        Object Current { get; }

        ValueTask<Boolean> MoveNextAsync();
    }


    public interface IAsyncEnumerator<out T> : IAsyncDisposable
    {
        /// <summary>Gets the element in the collection at the current position of the enumerator.</summary>
        T Current { get; }

        /// <summary>Gets the element in the collection at the current position of the enumerator.</summary>
        ValueTask<Boolean> MoveNextAsync();
    }
}