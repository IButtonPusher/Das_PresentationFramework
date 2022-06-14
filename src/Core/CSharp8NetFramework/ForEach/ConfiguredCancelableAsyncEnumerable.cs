using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct ConfiguredCancelableAsyncEnumerable<T>
    {
        /// <summary>Provides an awaitable async enumerator that enables cancelable iteration and configured awaits.</summary>
        [StructLayout(LayoutKind.Auto)]
        public readonly struct Enumerator
        {
            private readonly IAsyncEnumerator<T> _enumerator;

            private readonly Boolean _continueOnCapturedContext;

            /// <summary>Gets the element in the collection at the current position of the enumerator.</summary>
            public T Current => _enumerator.Current;

            internal Enumerator(IAsyncEnumerator<T> enumerator, Boolean continueOnCapturedContext)
            {
                _enumerator = enumerator;
                _continueOnCapturedContext = continueOnCapturedContext;
            }

            /// <summary>Advances the enumerator asynchronously to the next element of the collection.</summary>
            /// <returns>
            ///     A <see cref="T:System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable`1" /> that will complete with a result
            ///     of <c>true</c>
            ///     if the enumerator was successfully advanced to the next element, or <c>false</c> if the enumerator has
            ///     passed the end of the collection.
            /// </returns>
            public ConfiguredValueTaskAwaitable<Boolean> MoveNextAsync()
            {
                return _enumerator.MoveNextAsync().ConfigureAwait(_continueOnCapturedContext);
            }

            /// <summary>
            ///     Performs application-defined tasks associated with freeing, releasing, or
            ///     resetting unmanaged resources asynchronously.
            /// </summary>
            public ConfiguredValueTaskAwaitable DisposeAsync()
            {
                return _enumerator.DisposeAsync().ConfigureAwait(_continueOnCapturedContext);
            }
        }

        private readonly IAsyncEnumerable<T> _enumerable;

        private readonly CancellationToken _cancellationToken;

        private readonly Boolean _continueOnCapturedContext;

        internal ConfiguredCancelableAsyncEnumerable(IAsyncEnumerable<T> enumerable, 
                                                     Boolean continueOnCapturedContext,
                                                     CancellationToken cancellationToken)
        {
            _enumerable = enumerable;
            _continueOnCapturedContext = continueOnCapturedContext;
            _cancellationToken = cancellationToken;
        }

        /// <summary>Configures how awaits on the tasks returned from an async iteration will be performed.</summary>
        /// <param name="continueOnCapturedContext">Whether to capture and marshal back to the current context.</param>
        /// <returns>The configured enumerable.</returns>
        /// <remarks>
        ///     This will replace any previous value set by
        ///     <see cref="M:System.Runtime.CompilerServices.ConfiguredCancelableAsyncEnumerable`1.ConfigureAwait(System.Boolean)" />
        ///     for this iteration.
        /// </remarks>
        public ConfiguredCancelableAsyncEnumerable<T> ConfigureAwait(Boolean continueOnCapturedContext)
        {
            return new ConfiguredCancelableAsyncEnumerable<T>(_enumerable, continueOnCapturedContext,
                _cancellationToken);
        }

        /// <summary>
        ///     Sets the <see cref="T:System.Threading.CancellationToken" /> to be passed to
        ///     <see cref="M:System.Collections.Generic.IAsyncEnumerable`1.GetAsyncEnumerator(System.Threading.CancellationToken)" />
        ///     when iterating.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> to use.</param>
        /// <returns>The configured enumerable.</returns>
        /// <remarks>
        ///     This will replace any previous <see cref="T:System.Threading.CancellationToken" /> set by
        ///     <see
        ///         cref="M:System.Runtime.CompilerServices.ConfiguredCancelableAsyncEnumerable`1.WithCancellation(System.Threading.CancellationToken)" />
        ///     for this iteration.
        /// </remarks>
        public ConfiguredCancelableAsyncEnumerable<T> WithCancellation(CancellationToken cancellationToken)
        {
            return new ConfiguredCancelableAsyncEnumerable<T>(_enumerable, _continueOnCapturedContext,
                cancellationToken);
        }

        public Enumerator GetAsyncEnumerator()
        {
            return new Enumerator(_enumerable.GetAsyncEnumerator(_cancellationToken), _continueOnCapturedContext);
        }
    }
}