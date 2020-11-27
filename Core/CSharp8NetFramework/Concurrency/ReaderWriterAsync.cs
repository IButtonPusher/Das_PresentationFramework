using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#if !NET40
using TaskEx = System.Threading.Tasks.Task;

#endif

namespace System.Threading
{
    public partial class ReaderWriterAsync : IReaderWriterAsync
    {
        public ReaderWriterAsync()
        {
            _lockObj = new Object();
            _waiters = new Queue<ITaskWaiter>();
        }

      

        public Task<TResult> ReadAsync<TResult>(Func<TResult> action)
        {
            if (_isDisposed)
                return TaskEx.FromResult(default(TResult)!);

            lock (_lockObj)
            {
                if (_writerCount > 0 || _waiterCount > 0)
                {
                    _waiterCount++;
                    var waiter = new AsyncWaiter<TResult>(action, WorkerTypes.Reader);
                    _waiters.Enqueue(waiter);
                    return waiter;
                }

                _readerCount++;
            }

            try
            {
                var res = action();
                return TaskEx.FromResult(res);
            }
            finally
            {
                EndReaderImpl(action);
            }
        }

        public Task<TResult> ReadAsync<TParam, TResult>(
            TParam param, Func<TParam, Task<TResult>> action)
        {
            if (_isDisposed)
            {
                TResult res = default;
                return TaskEx.FromResult(res!);
            }

            lock (_lockObj)
            {
                if (_writerCount > 0 || _waiterCount > 0)
                {
                    _waiterCount++;
                    var waiter = new AsyncAwaiter<TParam, TResult>(param,
                        action, WorkerTypes.Reader);
                    _waiters.Enqueue(waiter);
                    return waiter;
                }

                _readerCount++;
            }

            try
            {
                var res = action(param);
                return res;
            }
            finally
            {
                EndReaderImpl(action);
            }
        }


        public Task<TResult> ReadAsync<TParam1, TResult>(TParam1 p1, Func<TParam1, TResult> action)
        {
            if (_isDisposed)
                return TaskEx.FromResult(default(TResult)!);

            lock (_lockObj)
            {
                if (_writerCount > 0 || _waiterCount > 0)
                {
                    _waiterCount++;
                    var waiter = new AsyncWaiter<TParam1, TResult>(p1, action, WorkerTypes.Reader);
                    _waiters.Enqueue(waiter);
                    return waiter;
                }

                _readerCount++;
            }

            try
            {
                var res = action(p1);
                return TaskEx.FromResult(res);
            }
            finally
            {
                EndReaderImpl(action);
            }
        }

        public Task<TResult> ReadAsync<TParam1, TParam2, TParam3, TResult>(
            TParam1 p1, 
            TParam2 p2, 
            TParam3 p3, 
            Func<TParam1, TParam2, TParam3, TResult> action)
        {
            if (_isDisposed)
                return TaskEx.FromResult(default(TResult)!);

            lock (_lockObj)
            {
                if (_writerCount > 0 || _waiterCount > 0)
                {
                    _waiterCount++;
                    var waiter = new AsyncWaiter<TParam1, TParam2, TParam3, TResult>(
                        p1, p2, p3, action, WorkerTypes.Reader);
                    _waiters.Enqueue(waiter);
                    return waiter;
                }

                _readerCount++;
            }

            try
            {
                var res = action(p1, p2, p3);
                return TaskEx.FromResult(res);
            }
            finally
            {
                EndReaderImpl(action);
            }
        }

        public Task<TResult> ReadAsync<TParam1, TParam2, TResult>(TParam1 p1, 
                                                                  TParam2 p2,
                                                                  Func<TParam1, TParam2, TResult> action)
        {
            if (_isDisposed)
                return TaskEx.FromResult(default(TResult)!);

            lock (_lockObj)
            {
                if (_writerCount > 0 || _waiterCount > 0)
                {
                    _waiterCount++;
                    var waiter = new AsyncWaiter<TParam1, TParam2, TResult>(p1, p2, action, WorkerTypes.Reader);
                    _waiters.Enqueue(waiter);
                    return waiter;
                }

                _readerCount++;
            }

            try
            {
                var res = action(p1, p2);
                return TaskEx.FromResult(res);
            }
            finally
            {
                EndReaderImpl(action);
            }
        }

        

        public async IAsyncEnumerable<TResult> ReadManyAsync<TResult>(Func<IEnumerable<TResult>> action)
        {
            Task waitFor;

            if (_isDisposed)
                yield break;

            lock (_lockObj)
            {
                if (_writerCount > 0 || _waiterCount > 0)
                {
                    _waiterCount++;
                    var waiter = new AsyncWaiter<Boolean>(() => true, WorkerTypes.IteratorReader);
                    _waiters.Enqueue(waiter);
                    waitFor = waiter;
                }
                else
                {
                    waitFor = TaskEx.CompletedTask;
                    _readerCount++;
                }
            }

            await waitFor;

            try
            {
                foreach (var a in action())
                {
                    if (_isDisposed)
                        yield break;
                    yield return a;
                }
            }
            finally
            {
                EndReaderImpl(action);
            }
        }

        public async IAsyncEnumerable<TResult> ReadManyAsync<TParam, TResult>(TParam p1,
                                                                              Func<TParam, IAsyncEnumerable<TResult>>
                                                                                  action)
        {
            Task waitFor;

            if (_isDisposed)
                yield break;

            lock (_lockObj)
            {
                if (_writerCount > 0 || _waiterCount > 0)
                {
                    _waiterCount++;
                    var waiter = new AsyncWaiter<Boolean>(() => true, WorkerTypes.IteratorReader);
                    _waiters.Enqueue(waiter);
                    waitFor = waiter;
                }
                else
                {
                    waitFor = TaskEx.CompletedTask;
                    _readerCount++;
                }
            }

            await waitFor;

            try
            {
                await foreach (var a in action(p1))
                {
                    if (_isDisposed)
                        yield break;
                    yield return a;
                }
            }
            finally
            {
                EndReaderImpl(action);
            }
        }


      

        public Task WriteAsync(Func<Task> asyncAction)
        {
            if (_isDisposed)
                return TaskEx.CompletedTask;

            lock (_lockObj)
            {
                if (_writerCount > 0 || _readerCount > 0 || _waiterCount > 0)
                {
                    _waiterCount++;
                    var waiter = new AsyncWaiter<Task>(
                        asyncAction, WorkerTypes.Writer);
                    _waiters.Enqueue(waiter);
                    return waiter;
                }

                _writerCount++;
            }

            try
            {
                var res = asyncAction();
                return res;
            }
            finally
            {
                EndWriterImpl();
            }
        }

        public Task<Int32> WriteAsync(Func<Int32> action)
        {
            if (_isDisposed)
                return TaskEx.FromResult(0);

            lock (_lockObj)
            {
                if (_writerCount > 0 || _readerCount > 0 || _waiterCount > 0)
                {
                    _waiterCount++;
                    var waiter = new AsyncWaiter<Int32>(action, WorkerTypes.Writer);
                    _waiters.Enqueue(waiter);
                    return waiter;
                }

                _writerCount++;
            }

            try
            {
                var res = action();
                return TaskEx.FromResult(res);
            }
            finally
            {
                EndWriterImpl();
            }
        }

        public Task<TResult> WriteAsync<TParam, TResult>(TParam param, 
                                                         Func<TParam, TResult> action)
        {
            if (_isDisposed)
                return TaskEx.FromResult(default(TResult)!);

            lock (_lockObj)
            {
                if (_writerCount > 0 || _readerCount > 0 || _waiterCount > 0)
                {
                    _waiterCount++;
                    var waiter = new AsyncWaiter<TParam, TResult>(param,
                        action, WorkerTypes.Writer);
                    _waiters.Enqueue(waiter);
                    return waiter;
                }

                _writerCount++;
            }

            try
            {
                var res = action(param);
                return TaskEx.FromResult(res);
            }
            finally
            {
                EndWriterImpl();
            }
        }

      

        public Task WriteAsync<TParam1, TParam2>(TParam1 p1, 
                                                 TParam2 p2, 
                                                 Action<TParam1, TParam2> action)
        {
            if (_isDisposed)
                return TaskEx.CompletedTask;

            lock (_lockObj)
            {
                if (_writerCount > 0 || _readerCount > 0 || _waiterCount > 0)
                {
                    _waiterCount++;
                    var waiter = new AsyncActionWaiter<TParam1, TParam2>(p1, p2,
                        action, WorkerTypes.Writer);
                    _waiters.Enqueue(waiter);
                    return waiter;
                }

                _writerCount++;
            }

            try
            {
                action(p1, p2);
                return TaskEx.CompletedTask;
            }
            finally
            {
                EndWriterImpl();
            }
        }

        //public Task<Int32> WriteAsync<TParam>(TParam param,
        //                                      Func<TParam, Int32> action)
        //{
        //    if (_isDisposed)
        //        return TaskEx.FromResult(0);

        //    lock (_lockObj)
        //    {
        //        if (_writerCount > 0 || _readerCount > 0 || _waiterCount > 0)
        //        {
        //            _waiterCount++;
        //            var waiter = new AsyncWaiter<TParam, Int32>(param,
        //                action, WorkerTypes.Writer);
        //            _waiters.Enqueue(waiter);
        //            return waiter;
        //        }

        //        _writerCount++;
        //    }

        //    try
        //    {
        //        var res = action(param);
        //        return TaskEx.FromResult(res);
        //    }
        //    finally
        //    {
        //        EndWriterImpl();
        //    }
        //}

        public Task<TResult> WriteAsync<TParam, TResult>(TParam param, 
                                                         Func<TParam, Task<TResult>> action)
        {
            if (_isDisposed)
                return TaskEx.FromResult(default(TResult)!);

            lock (_lockObj)
            {
                if (_writerCount > 0 || _readerCount > 0 || _waiterCount > 0)
                {
                    _waiterCount++;
                    var waiter = new AsyncAwaiter<TParam, TResult>(param,
                        action, WorkerTypes.Writer);
                    _waiters.Enqueue(waiter);
                    return waiter;
                }

                _writerCount++;
            }

            try
            {
                var res = action(param);
                return res;
            }
            finally
            {
                EndWriterImpl();
            }
        }

        public Task<Int32> WriteAsync<TParam>(TParam param,
                                              Func<TParam, Task<Int32>> action)
        {
            if (_isDisposed)
                return TaskEx.FromResult(0);

            lock (_lockObj)
            {
                if (_writerCount > 0 || _readerCount > 0 || _waiterCount > 0)
                {
                    _waiterCount++;
                    var waiter = new AsyncAwaiter<TParam, Int32>(param,
                        action, WorkerTypes.Writer);
                    _waiters.Enqueue(waiter);
                    return waiter;
                }

                _writerCount++;
            }

            try
            {
                var res = action(param);
                return res;
            }
            finally
            {
                EndWriterImpl();
            }
        }

        public void Clear()
        {
            lock (_lockObj)
            {
                while (_waiters.Count > 0)
                {
                    var tooLate = _waiters.Dequeue();
                    tooLate.Cancel();
                }

                _waiterCount = 0;
            }
        }

        public void Dispose()
        {
            _isDisposed = true;

            ITaskWaiter[] allWaiters;
            lock (_lockObj)
            {
                allWaiters = _waiters.ToArray();
                _waiters.Clear();
            }

            for (var c = 0; c < allWaiters.Length; c++)
            {
                var w = allWaiters[c];
                w.Cancel();
            }
        }


        private void ExecuteWaiter(ITaskWaiter worker)
        {
            worker.Execute();

            if (worker.Status != TaskStatus.RanToCompletion)
                return;

            lock (_lockObj)
            {
                switch (worker.WorkerType)
                {
                    case WorkerTypes.Writer:
                        _writerCount--;
                        break;
                    case WorkerTypes.Reader:
                        _readerCount--;
                        break;
                    case WorkerTypes.IteratorReader:
                        return;
                }
            }

            StartNextWorkers();
        }


        private void StartNextWorkers()
        {
            while (TryGetNext(out var nextWorker))
            {
                var worker = nextWorker;
                var todo = new WaiterAction(worker, ExecuteWaiter);
                var running = TaskEx.Run(todo.Execute);
                running.ConfigureAwait(false);
            }
        }

        private Boolean TryGetNext(out ITaskWaiter nextWorker)
        {
            lock (_lockObj)
            {
                if (_waiterCount == 0 || _waiters.Count == 0)
                {
                    nextWorker = default!;
                    return false;
                }

                nextWorker = _waiters.Peek();

                switch (nextWorker.WorkerType)
                {
                    case WorkerTypes.Writer when _readerCount == 0 && _writerCount == 0:
                        _writerCount++;
                        nextWorker = _waiters.Dequeue();
                        break;
                    case WorkerTypes.Reader when _writerCount == 0:
                    case WorkerTypes.IteratorReader when _writerCount == 0:
                        _readerCount++;
                        nextWorker = _waiters.Dequeue();
                        break;

                    default:
                        return false;
                }

                _waiterCount--;
            }

            return true;
        }

        private readonly Object _lockObj;

        private readonly Queue<ITaskWaiter> _waiters;
        private Boolean _isDisposed;
        private Int32 _readerCount;
        private Int32 _waiterCount;
        private Int32 _writerCount;
    }
}