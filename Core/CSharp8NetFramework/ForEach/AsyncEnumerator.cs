using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable All

#if !NET40
using TaskEx = System.Threading.Tasks.Task;
#endif

#pragma warning disable 8625
#pragma warning disable 8604
#pragma warning disable 8603
#pragma warning disable 8601
#pragma warning disable 8618

namespace AsyncResults.Enumerable
{
    public abstract class AsyncEnumerator
    {
        public static IAsyncEnumerator Empty { get; }

        public static IAsyncEnumerator<T> GetEmpty<T>() => AsyncEnumerator<T>.Empty;
    }

    public class AsyncEnumeratorWithState<TItem, TState> : CurrentValueContainer<TItem>, IAsyncEnumerator,
                                                           IAsyncEnumerator<TItem>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="enumerationFunction">A function that enumerates items in a collection asynchronously</param>
        /// <param name="state">Any state object that is passed to the <paramref name="enumerationFunction" /></param>
        /// <param name="onDispose">Optional action that gets invoked on Dispose()</param>
        public AsyncEnumeratorWithState(
            Func<AsyncEnumerator<TItem>.Yield, TState, Task> enumerationFunction,
            TState state,
            Action<TState> onDispose = null)
        {
            _enumerationFunction = enumerationFunction ?? throw new ArgumentNullException(nameof(enumerationFunction));
            _onDisposeAction = onDispose;
            State = state;

            // If dispose action has not been defined and enumeration has not been started, there is nothing to finilize.
            if (onDispose == null)
                GC.SuppressFinalize(this);
        }

        Object IAsyncEnumerator.Current => Current;

        /// <summary>
        ///     Advances the enumerator to the next element of the collection asynchronously
        /// </summary>
        /// <returns>
        ///     Returns a Task that does transition to the next element. The result of the task is True if the enumerator was
        ///     successfully advanced to the next element, or False if the enumerator has passed the end of the collection.
        /// </returns>
        public virtual ValueTask<Boolean> MoveNextAsync()
        {
            if (_enumerationFunction == null)
                return new ValueTask<Boolean>(false);

            if (_yield == null)
                _yield = new AsyncEnumerator<TItem>.Yield(this, MasterCancellationToken);

            var moveNextCompleteTask = _yield.OnMoveNext();

            if (_enumerationTask == null)
            {
                // Register for finalization, which might be needed if caller
                // doesn't not finish the enumeration and does not call Dispose().
                GC.ReRegisterForFinalize(this);

                _enumerationTask =
                    _enumerationFunction(_yield, State)
                        .ContinueWith(OnEnumerationCompleteAction, this, TaskContinuationOptions.ExecuteSynchronously);
            }

            return moveNextCompleteTask;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources
        /// </summary>
        public void Dispose()
        {
            DisposeAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            var enumTask = _enumerationTask;

            Dispose(manualDispose: true);

            if (enumTask != null)
            {
                try
                {
                    await enumTask;
                }
                catch
                {
                }
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Gets the element in the collection at the current position of the enumerator
        /// </summary>
        public virtual TItem Current
        {
            get
            {
                if (!HasCurrentValue)
                    throw new InvalidOperationException(
                        "Call MoveNextAsync() or MoveNext() before accessing the Current item");
                return CurrentValue;
            }
        }

        /// <summary>
        ///     Tells if enumeration is complete. Returns True only after MoveNextAsync returns False.
        /// </summary>
        public Boolean IsEnumerationComplete => _yield != null && _yield.IsComplete;

        /// <summary>
        ///     A user state that gets passed into the enumeration function.
        /// </summary>
        protected TState State { get; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources
        /// </summary>
        /// <param name="manualDispose">True if called from Dispose() method, otherwise False - called by GC</param>
        protected virtual void Dispose(Boolean manualDispose)
        {
            if (manualDispose)
            {
                _yield?.SetCanceled();
            }
            else if (_yield != null && !_yield.IsComplete)
            {
                var yield = _yield;
                TaskEx.Run(() => yield.SetCanceled()); // don't block the GC thread
            }

            _enumerationTask = null;
            _yield = null;

            _onDisposeAction?.Invoke(State);
            _onDisposeAction = null;
            _enumerationFunction = null;
        }

        private static void OnEnumerationComplete(Task task, Object state)
        {
            var enumerator = (AsyncEnumeratorWithState<TItem, TState>) state;

            // When en enumeration is complete, there is nothing to dispose.
            GC.SuppressFinalize(enumerator);

            if (task.IsFaulted)
            {
                // ReSharper disable once PossibleNullReferenceException
                if (task.Exception.GetBaseException() is AsyncEnumerationCanceledException)
                {
                    enumerator._yield?.SetCanceled();
                }
                else
                {
                    enumerator._yield?.SetFailed(task.Exception);
                }
            }
            else if (task.IsCanceled)
            {
                enumerator._yield?.SetCanceled();
            }
            else
            {
                enumerator._yield?.SetComplete();
            }
        }

        /// <summary>
        ///     Finalizer
        /// </summary>
        ~AsyncEnumeratorWithState()
        {
            Dispose(manualDispose: false);
        }

        private static readonly Action<Task, Object> OnEnumerationCompleteAction = OnEnumerationComplete;

        private Func<AsyncEnumerator<TItem>.Yield, TState, Task> _enumerationFunction;
        private Task _enumerationTask;
        private Action<TState> _onDisposeAction;
        private AsyncEnumerator<TItem>.Yield _yield;

        internal CancellationToken MasterCancellationToken;
    }

    public class AsyncEnumerator<T> : AsyncEnumeratorWithState<T, AsyncEnumerator<T>.NoStateAdapter>
    {
        public AsyncEnumerator(Func<Yield, Task> enumerationFunction, Action onDispose = null)
            : base(
                enumerationFunction: NoStateAdapter.Enumerate,
                onDispose: onDispose == null ? null : NoStateAdapter.OnDispose,
                state: new NoStateAdapter
                {
                    EnumerationFunction = enumerationFunction,
                    DisposeAction = onDispose
                })
        {
        }

#pragma warning disable 108,114
        public static readonly IAsyncEnumerator<T> Empty = new EmptyAsyncEnumerator<T>();
#pragma warning restore 108,114


        public sealed class Yield
        {
            internal Yield(CurrentValueContainer<T> currentValueContainer, CancellationToken cancellationToken)
            {
                _currentValueContainer = currentValueContainer;
                CancellationToken = cancellationToken;
            }


            public CancellationToken CancellationToken { get; private set; }

            internal Boolean IsComplete { get; private set; }

            /// <summary>
            ///     Stops iterating items in the collection (similar to 'yield break' statement)
            /// </summary>
            /// <exception cref="AsyncEnumerationCanceledException">Always throws this exception to stop the enumeration task</exception>
            public void Break()
            {
                SetComplete();
                throw new AsyncEnumerationCanceledException();
            }

            internal ValueTask<Boolean> OnMoveNext()
            {
                if (!IsComplete)
                {
                    TaskCompletionSource.Reset(ref _moveNextCompleteTcs);
                    _resumeEnumerationTcs?.TrySetResult(true);
                }

                return new ValueTask<Boolean>(_moveNextCompleteTcs.Task);
            }


#pragma warning disable AsyncMethodMustTakeCancellationToken // Does not take a CancellationToken by design
            public Task ReturnAsync(T item)
#pragma warning restore AsyncMethodMustTakeCancellationToken
            {
                TaskCompletionSource.Reset(ref _resumeEnumerationTcs);
                _currentValueContainer.CurrentValue = item;
                _moveNextCompleteTcs.TrySetResult(true);
                return _resumeEnumerationTcs.Task;
            }

            internal void SetCanceled()
            {
                IsComplete = true;
                if (!CancellationToken.IsCancellationRequested)
                    CancellationToken = CancellationTokenEx.Canceled;
                _resumeEnumerationTcs?.TrySetException(new AsyncEnumerationCanceledException());
                _moveNextCompleteTcs?.TrySetCanceled();
            }

            internal void SetComplete()
            {
                IsComplete = true;
                _moveNextCompleteTcs.TrySetResult(false);
            }

            internal void SetFailed(Exception ex)
            {
                IsComplete = true;
                _moveNextCompleteTcs?.TrySetException(ex.GetBaseException());
            }

            private CurrentValueContainer<T> _currentValueContainer;
            private TaskCompletionSource<Boolean> _moveNextCompleteTcs;

            private TaskCompletionSource<Boolean>
                _resumeEnumerationTcs; // Can be of any type - there is no non-generic version of the TaskCompletionSource
        }


        [EditorBrowsable(EditorBrowsableState.Never)]
        public struct NoStateAdapter
        {
            internal Func<Yield, Task> EnumerationFunction;
            internal Action DisposeAction;

            internal static readonly Func<Yield, NoStateAdapter, Task> Enumerate = EnumerateWithNoStateAdapter;
            internal static readonly Action<NoStateAdapter> OnDispose = OnDisposeAdapter;

            private static Task EnumerateWithNoStateAdapter(Yield yield, NoStateAdapter state) =>
                state.EnumerationFunction(yield);

            private static void OnDisposeAdapter(NoStateAdapter state) => state.DisposeAction?.Invoke();
        }
    }
}