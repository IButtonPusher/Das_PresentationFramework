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

            _disposeClearCancellation = new CancellationTokenSource();
        }


        public void Clear()
        {
            _disposeClearCancellation.Cancel(false);

            lock (_lockObj)
            {
                if (_writerCount > 0 || _readerCount > 0)
                {
                }

                while (_waiters.Count > 0)
                {
                    var tooLate = _waiters.Dequeue();
                    tooLate.Cancel();
                }

                _waiterCount = 0;

                _disposeClearCancellation = new CancellationTokenSource();
            }
        }

        public void Dispose()
        {
            _disposeClearCancellation.Cancel(false);

            Interlocked.Add(ref _disposedCount, 1);


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
            worker.ExecuteBlocking();

            if (worker.Status != TaskStatus.RanToCompletion)
            {
                //   return;
            }

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

        protected Boolean _isDisposed => _disposedCount > 0;

        private readonly Object _lockObj;

        private readonly Queue<ITaskWaiter> _waiters;

        private CancellationTokenSource _disposeClearCancellation;
        private Int64 _disposedCount;
        private Int32 _readerCount;
        private Int32 _waiterCount;
        private Int32 _writerCount;
    }
}
