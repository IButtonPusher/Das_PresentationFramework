using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    public interface IAsyncEnumerable
    {
        /// <summary>
        ///     Creates an enumerator that iterates through a collection asynchronously
        /// </summary>
        /// <param name="cancellationToken">
        ///     A cancellation token to cancel creation of the enumerator in case if it takes a lot of
        ///     time
        /// </param>
        /// <returns>Returns a task with the created enumerator as result on completion</returns>
        IAsyncEnumerator GetAsyncEnumerator(CancellationToken cancellationToken = default);
    }

    public interface IAsyncEnumerable<out T> //: IAsyncEnumerable
    {
        /// <summary>Returns an enumerator that iterates asynchronously through the collection.</summary>
        /// <param name="cancellationToken">
        ///     A <see cref="T:System.Threading.CancellationToken" /> that may be used to cancel the
        ///     asynchronous iteration.
        /// </param>
        /// <returns>An enumerator that can be used to iterate asynchronously through the collection.</returns>
        IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default);
    }
}