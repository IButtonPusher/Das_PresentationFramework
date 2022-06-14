using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
#if !NET40
using TaskEx = System.Threading.Tasks.Task;
#endif

namespace CSharp8NetFramework.Concurrency.Waiters
{
    public class AsyncEnumerableWaiter<TParam1, TParam2, TResult> :
        AsyncTaskCompletionSource<IAsyncEnumerable<TResult>>,
        IAsyncEnumerableWaiter<TResult>,
        IAsyncEnumerable<TResult>
    {
        public AsyncEnumerableWaiter(TParam1 p1,
                                     TParam2 p2,
                                     RunManyAsync<TParam1, TParam2, TResult> action,
                                     WorkerTypes workerType,
                                     CancellationToken cancellationToken)
        {
            _p1 = p1;
            _p2 = p2;
            _action = action;
            _cancellationToken = cancellationToken;
            WorkerType = workerType;


            _faker = new FakeEnumerable();
        }

        public IAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return _faker;
        }

        public async IAsyncEnumerable<TResult> GetResultsAsync()
        {
            var res = new List<TResult>();

            try
            {
                await foreach (var r in _action(_p1, _p2).ConfigureAwait(false))
                {
                    res.Add(r);
                    yield return r;
                    if (_isCancelled || _cancellationToken.IsCancellationRequested)
                        yield break;
                }
            }
            finally
            {
                _faker.SetItems(res);
                TrySetResult(_faker);
            }
        }

        public TaskStatus Status => Task.Status;

        public WorkerTypes WorkerType { get; }

        public void Cancel()
        {
            _isCancelled = true;
        }

        public void BeginExecute()
        {
            ExecuteImpl().ConfigureAwait(false);
        }

        public void ExecuteBlocking()
        {
            ExecuteImpl().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private async Task ExecuteImpl()
        {
            await TaskEx.Yield();
            await foreach (var _ in GetResultsAsync().ConfigureAwait(false))
            {
            }
        }

        private readonly RunManyAsync<TParam1, TParam2, TResult> _action;
        private readonly CancellationToken _cancellationToken;
        private readonly FakeEnumerable _faker;
        private readonly TParam1 _p1;
        private readonly TParam2 _p2;

        private Boolean _isCancelled;

        private class FakeEnumerable : List<TResult>,
                                       IAsyncEnumerable<TResult>,
                                       IAsyncEnumerator<TResult>
        {
            public FakeEnumerable()
            {
                _currentValue = default;
                _completion = new AsyncTaskCompletionSource<Boolean>();
            }

            //public FakeEnumerable(IEnumerable<TResult> items)
            //    : base(items)
            //{
            //    _currentValue = default;
            //}

            public IAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return this;
            }

            public async ValueTask DisposeAsync()
            {
                await TaskEx.CompletedTask;
            }

            public TResult Current
            {
                get
                {
                    if (_hasCurrentValue)
                        throw new InvalidOperationException(
                            "Call MoveNextAsync() or MoveNext() before accessing the Current item");
                    return _currentValue!;
                }
            }

            public async ValueTask<Boolean> MoveNextAsync()
            {
                await _completion.Task.ConfigureAwait(false);

                if (++_currentIndex < Count)
                {
                    _currentValue = this[_currentIndex];
                    _hasCurrentValue = true;
                    return true;
                }

                return false;
            }

            public void SetItems(IEnumerable<TResult> items)
            {
                AddRange(items);
                _completion.TrySetResult(true);
            }

            private readonly AsyncTaskCompletionSource<Boolean> _completion;

            private Int32 _currentIndex;
            private TResult? _currentValue;
            private Boolean _hasCurrentValue;
        }
    }

    //public delegate IAsyncEnumerable<TResult> RunMany<in TParam1, in TParam2, out TResult>(TParam1 p1, 
    //    TParam2 items);

    //public delegate IAsyncEnumerable<TResult> RunMany<in TParam1, in TParam2, out TResult>(TParam1 p1,
    //    TParam2 p2);
}
