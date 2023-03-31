using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#if !NET40
using TaskEx = System.Threading.Tasks.Task;
#endif


namespace CSharp8NetFramework.Concurrency.Waiters
{
    public class AsyncEnumerableWaiter2<TParam1, TParam2, TResult> :
        IAsyncEnumerableWaiter<TResult>,
        IAsyncEnumerable<TResult>,
        IAsyncEnumerator<TResult>
    {
        public AsyncEnumerableWaiter2(TParam1 p1,
                                      TParam2 p2,
                                      RunMany<TParam1, TParam2, TResult> action,
                                      WorkerTypes workerType,
                                      CancellationToken cancellationToken)
        {
            _p1 = p1;
            _p2 = p2;
            _action = action;
            _cancellationToken = cancellationToken;
            WorkerType = workerType;

            _currentItemCompletionLock = new Object();
            _currentItemCompletion = new AsyncTaskCompletionSource<TResult>();
            _currentItemRequest = new AsyncTaskCompletionSource<Boolean>();

            Status = TaskStatus.WaitingToRun;
        }

        public IAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return this;
        }

        public IAsyncEnumerable<TResult> GetResultsAsync()
        {
            return this;
            //var res = new List<TResult>();

            //try
            //{
            //    await foreach (var r in _action(_p1, _p2).ConfigureAwait(false))
            //    {
            //        res.Add(r);
            //        yield return r;
            //        if (_isCancelled || _cancellationToken.IsCancellationRequested)
            //            yield break;
            //    }
            //}
            //finally
            //{
            //    _faker.SetItems(res);
            //    TrySetResult(_faker);
            //}
        }

        public TaskStatus Status { get; private set; }

        public WorkerTypes WorkerType { get; }

        public void Cancel()
        {
            _isFinished = true;
            lock (_currentItemCompletionLock)
            {
                _currentItemRequest.TrySetResult(false);
                _currentItemCompletion.TrySetResult(default!);
            }
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
            foreach (var r in _action(_p1, _p2))
            {
                AsyncTaskCompletionSource<Boolean> waiting;
                lock (_currentItemCompletionLock)
                {
                    waiting = _currentItemRequest;
                    _currentItemCompletion = new AsyncTaskCompletionSource<TResult>();
                }

                await waiting.Task.ConfigureAwait(false);

                if (_cancellationToken.IsCancellationRequested)
                    _isFinished = true;
                
                if (_isFinished)
                    break;

                _currentItemCompletion.TrySetResult(r);
            }
        }

        private readonly RunMany<TParam1, TParam2, TResult> _action;
        private readonly CancellationToken _cancellationToken;
        
        private readonly TParam1 _p1;
        private readonly TParam2 _p2;

        private Boolean _isFinished;

        private AsyncTaskCompletionSource<TResult> _currentItemCompletion;
        private AsyncTaskCompletionSource<Boolean> _currentItemRequest;
        private readonly Object _currentItemCompletionLock;


        public async ValueTask DisposeAsync()
        {
            Cancel();

            await TaskEx.CompletedTask; }

        public TResult Current => _currentItemCompletion.Task.Result;

        public async ValueTask<Boolean> MoveNextAsync()
        {
            if (_isFinished)
                return false;

            lock (_currentItemCompletionLock)
            {
                _currentItemRequest.SetResult(true);
            }

            await _currentItemCompletion.Task;

            lock (_currentItemCompletionLock)
            {
                _currentItemRequest = new AsyncTaskCompletionSource<Boolean>();
            }

            return true;
        }
    }

    //public delegate IAsyncEnumerable<TResult> RunMany<in TParam1, in TParam2, out TResult>(TParam1 p1, 
    //    TParam2 items);

    //public delegate IAsyncEnumerable<TResult> RunMany<in TParam1, in TParam2, out TResult>(TParam1 p1,
    //    TParam2 p2);
}
