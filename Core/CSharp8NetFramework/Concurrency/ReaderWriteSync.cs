using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// ReSharper disable UnusedParameter.Local

namespace System.Threading
{
    public partial class ReaderWriterAsync
    {
        public T Read<T>(Func<T> action)
        {
            if (!ObtainSyncReadImpl(action))
                return default!;

            try
            {
                var res = action();
                return res;
            }
            finally
            {
                EndReaderImpl(action);
            }
        }

        public IDisposable Read()
        {
            if (!ObtainSyncReadImpl())
                return default!;

            return new SyncReader(EndReaderImpl);
        }

        public TResult Read<TInput, TResult>(TInput input, Func<TInput, TResult> action)
        {
            if (!ObtainSyncReadImpl(action))
                return default!;

            try
            {
                var res = action(input);
                return res;
            }
            finally
            {
                EndReaderImpl(action);
            }
        }

        public IEnumerable<TResult> ReadMany<TParam, TResult>(TParam p1,
                                                              Func<TParam, IEnumerable<TResult>> action)
        {
            if (!ObtainSyncReadImpl(action))
                yield break;

            try
            {
                foreach (var r in action(p1))
                {
                    if (_isDisposed)
                        yield break;

                    yield return r;
                }
            }
            finally
            {
                EndReaderImpl(action);
            }
        }


        public IEnumerable<T> ReadMany<T>(Func<IEnumerable<T>> action)
        {
            if (!ObtainSyncReadImpl(action))
                yield break;

            try
            {
                foreach (var r in action())
                {
                    if (_isDisposed)
                        yield break;

                    yield return r;
                }
            }
            finally
            {
                EndReaderImpl(action);
            }
        }

        public Int32 Write(Func<Int32> action)
        {
            if (!ObtainSyncWriteImpl(action))
                return 0;

            try
            {
                var res = action();
                return res;
            }
            finally
            {
                EndWriterImpl();
            }
        }

        public Int32 Write<TParam1>(TParam1 p1, Func<TParam1, Int32> action)
        {
            if (!ObtainSyncWriteImpl(action))
                return 0;

            try
            {
                var res = action(p1);
                return res;
            }
            finally
            {
                EndWriterImpl();
            }
        }

        public Int32 Write<TParam1, TParam2>(TParam1 p1, TParam2 p2,
                                             Func<TParam1, TParam2, Int32> action)
        {
            if (!ObtainSyncWriteImpl(action))
                return 0;

            try
            {
                var res = action(p1, p2);
                return res;
            }
            finally
            {
                EndWriterImpl();
            }
        }

        public void Write(Action action)
        {
            if (!ObtainSyncWriteImpl(action))
                return;

            try
            {
                action();
            }
            finally
            {
                EndWriterImpl();
            }
        }

        private void EndReaderImpl(Delegate action)
        {
            EndReaderImpl();
        }

        private void EndReaderImpl()
        {
            lock (_lockObj)
            {
                _readerCount--;
            }

            StartNextWorkers();
        }

        private void EndWriterImpl()
        {
            lock (_lockObj)
            {
                _writerCount--;
            }

            StartNextWorkers();
        }

        private Boolean ObtainSyncReadImpl(Delegate action)
        {
            return ObtainSyncReadImpl();
        }

        private Boolean ObtainSyncReadImpl()
        {
            if (_isDisposed)
                return false;

            SyncWaiter waiter;

            var spinCount = 0;

            lock (_lockObj)
            {
                var dothProceed = _writerCount == 0 && _waiterCount == 0;

                if (dothProceed)
                {
                    _readerCount++;
                    return true;
                }

                waiter = new SyncWaiter(WorkerTypes.Reader);
                _waiters.Enqueue(waiter);
                _waiterCount++;
            }

            while (waiter.Status != TaskStatus.WaitingToRun)
                if (++spinCount < 15)
                    Thread.Yield();
                else
                    Thread.Sleep(1);

            return true;
        }

        private Boolean ObtainSyncWriteImpl(Delegate action)
        {
            if (_isDisposed)
                return false;
            SyncWaiter waiter;

            var spinCount = 0;

            lock (_lockObj)
            {
                var dothProceed = _writerCount == 0
                                  && _waiterCount == 0 && _readerCount == 0;

                if (dothProceed)
                {
                    _writerCount++;
                    return true;
                }

                waiter = new SyncWaiter(WorkerTypes.Writer);
                _waiters.Enqueue(waiter);
                _waiterCount++;
            }

            while (waiter.Status != TaskStatus.WaitingToRun)
                if (++spinCount < 15)
                    Thread.Yield();
                else
                    Thread.Sleep(1);

            return true;
        }
    }
}