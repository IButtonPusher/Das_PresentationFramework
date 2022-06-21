using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace System.Threading
{
    public partial class ReaderWriterAsync
    {
        public T Read<T>(Func<T> action)
        {
            if (!ObtainSyncReadImpl())
                return default!;

            try
            {
                var res = action();
                return res;
            }
            finally
            {
                EndReaderImpl();
            }
        }

        public IDisposable Read()
        {
            if (!ObtainSyncReadImpl())
                return default!;

            return new SyncReader(EndReaderImpl);
        }

        public TResult Read<TInput1, TInput2, TResult>(TInput1 input,
                                                       TInput2 input2,
                                                       Func<TInput1, TInput2, TResult> action)
        {
            if (!ObtainSyncReadImpl())
                return default!;

            try
            {
                var res = action(input, input2);
                return res;
            }
            finally
            {
                EndReaderImpl();
            }
        }

        public TResult Read<TInput, TResult>(TInput input,
                                             Func<TInput, TResult> action)
        {
            if (!ObtainSyncReadImpl())
                return default!;

            try
            {
                var res = action(input);
                return res;
            }
            finally
            {
                EndReaderImpl();
            }
        }

        public IEnumerable<TResult> ReadMany<TParam, TResult>(TParam p1,
                                                              Func<TParam, IEnumerable<TResult>> action)
        {
            if (!ObtainSyncReadImpl())
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
                EndReaderImpl();
            }
        }


        public IEnumerable<T> ReadMany<T>(Func<IEnumerable<T>> action)
        {
            if (!ObtainSyncWriteImpl())
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
                EndWriterImpl();
            }
        }


        public Int32 Write(Func<Int32> action)
        {
            if (!ObtainSyncWriteImpl())
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

        public TResult Write<TParam1, TResult>(TParam1 p1,
                                               Func<TParam1, TResult> action)
        {
            if (!ObtainSyncWriteImpl())
                return default!;

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

        public void Write<TParam1, TParam2, TParam3>(TParam1 p1,
                                                     TParam2 p2,
                                                     TParam3 p3,
                                                     Action<TParam1, TParam2, TParam3> action)
        {
            if (!ObtainSyncWriteImpl())
                return;

            try
            {
                action(p1, p2, p3);
            }
            finally
            {
                EndWriterImpl();
            }
        }

        public Int32 Write<TParam1, TParam2>(TParam1 p1,
                                             TParam2 p2,
                                             Func<TParam1, TParam2, Int32> action)
        {
            if (!ObtainSyncWriteImpl())
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

        public void Write<TParam1, TParam2>(TParam1 p1,
                                            TParam2 p2,
                                            Action<TParam1, TParam2> action)
        {
            if (!ObtainSyncWriteImpl())
                return;

            try
            {
                action(p1, p2);
            }
            finally
            {
                EndWriterImpl();
            }
        }

        public void Write(Action action)
        {
            if (!ObtainSyncWriteImpl())
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

        public Int32 Write<TParam1, TParam2, TParam3>(TParam1 p1,
                                                      TParam2 p2,
                                                      TParam3 p3,
                                                      Func<TParam1, TParam2, TParam3, Int32> action)
        {
            if (!ObtainSyncWriteImpl())
                return 0;

            try
            {
                var res = action(p1, p2, p3);
                return res;
            }
            finally
            {
                EndWriterImpl();
            }
        }

        private void EndReaderImpl()
        {
            lock (_lockObj)
            {
                _readerCount--;

                if (_readerCount < 0)
                    throw new InvalidOperationException();
            }

            StartNextWorkers();
        }

        private void EndWriterImpl()
        {
            lock (_lockObj)
            {
                _writerCount--;

                if (_writerCount < 0)
                    throw new InvalidOperationException();
            }

            StartNextWorkers();
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

        private Boolean ObtainSyncWriteImpl()
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
