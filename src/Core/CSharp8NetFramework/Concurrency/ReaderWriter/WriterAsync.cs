using System;
using System.Threading.Tasks;

#if !NET40
using TaskEx = System.Threading.Tasks.Task;
#endif

namespace System.Threading
{
    public partial class ReaderWriterAsync
    {
        public async Task WriteTaskAsync(Func<Task> asyncAction)
        {
            if (_isDisposed)
                return;


            if (TryGetWriterWaiter(
                (r,
                 _) => new AsyncVoidTaskWaiter(asyncAction, r),
                ref _writerCount,
                out var waiter))
            {
                await waiter.ExecuteAsync();
                return;
            }

            try
            {
                await asyncAction();
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

            if (TryGetWriterWaiter((r,
                                    c) => new FuncWaiter<Int32>(action, r, c),
                ref _writerCount,
                out var waiter))
                return waiter;

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

            if (TryGetWriterWaiter((r,
                                    c) => new FuncWaiter<TParam, TResult>(param, action, r, c),
                ref _writerCount,
                out var waiter))
                return waiter;
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


        public Task<TResult> WriteAsync<TParam1, TParam2, TResult>(TParam1 p1,
                                                                   TParam2 p2,
                                                                   Func<TParam1, TParam2, TResult> action)
        {
            if (_isDisposed)
                return TaskEx.FromResult(default(TResult)!);

            if (TryGetWriterWaiter((r,
                                    c) =>
                    new FuncWaiter<TParam1, TParam2, TResult>(p1, p2, action, r, c),
                ref _writerCount,
                out var waiter))
                return waiter;


            try
            {
                var res = action(p1, p2);
                return TaskEx.FromResult(res);
            }
            finally
            {
                EndWriterImpl();
            }
        }


        public Task WriteAsync<TParam1>(TParam1 p1,
                                        Action<TParam1> action)
        {
            if (_isDisposed)
                return TaskEx.CompletedTask;

            if (TryGetWriterWaiter((_,
                                    _) =>
                    new ActionWaiter<TParam1>(p1,
                        action, WorkerTypes.Writer),
                ref _writerCount,
                out var waiter))
            {
                return waiter;
            }


            try
            {
                action(p1);
                return TaskEx.CompletedTask;
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

            if (TryGetWriterWaiter((w,
                                    _) =>
                    new ActionWaiter<TParam1, TParam2>(p1, p2,
                        action, w),
                ref _writerCount,
                out var waiter))
            {
                return waiter;
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

        public Task WriteAsync<TParam1, TParam2, TParam3>(TParam1 p1,
                                                          TParam2 p2,
                                                          TParam3 p3,
                                                          Action<TParam1, TParam2, TParam3> action)
        {
            if (_isDisposed)
                return TaskEx.CompletedTask;


            if (TryGetWriterWaiter((w,
                                    _) =>
                    new ActionWaiter<TParam1, TParam2, TParam3>(p1, p2, p3,
                        action, w),
                ref _writerCount,
                out var waiter))
            {
               return waiter;
            }


            try
            {
                action(p1, p2, p3);
                return TaskEx.CompletedTask;
            }
            finally
            {
                EndWriterImpl();
            }
        }

       

        public async Task WriteTaskAsync<TParam1, TParam2, TParam3, TParam4>(TParam1 p1,
                                                                             TParam2 p2,
                                                                             TParam3 p3,
                                                                             TParam4 p4,
                                                                             Func<TParam1, TParam2, TParam3, TParam4,
                                                                                 Task> action)
        {
            if (_isDisposed)
                return;

            if (TryGetWriterWaiter((r,
                                    _) =>
                    new AsyncVoidTaskWaiter<TParam1, TParam2, TParam3, TParam4>(p1, p2, p3, p4,
                        action, r),
                ref _writerCount,
                out var waiter))
            {
                await waiter.ExecuteAsync();
                return;
            }


            try
            {
                await action(p1, p2, p3, p4);
            }
            finally
            {
                EndWriterImpl();
            }
        }

        public Task WriteAsync<TParam1, TParam2>(TParam1 p1,
                                                 TParam2 p2,
                                                 Func<TParam1, TParam2, Task> action)
        {
            if (_isDisposed)
                return TaskEx.CompletedTask;

            if (TryGetWriterWaiter((w,
                                    _) => new AsyncVoidTaskWaiter<TParam1, TParam2>(p1, p2,
                    action, w),
                ref _writerCount,
                out var waiter))
                return waiter;


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

        public Task<TResult> WriteAsync<T1, T2, T3, TResult>(T1 p1,
                                                             T2 p2,
                                                             T3 p3,
                                                             Func<T1, T2, T3, TResult> action)
        {
           if (_isDisposed)
              return TaskEx.FromResult<TResult>(default!);


           if (TryGetWriterWaiter((w,
                                   c) =>
                 new FuncWaiter<T1, T2, T3,  TResult>(p1, p2, p3,
                    action, w, c),
              ref _writerCount,
              out var waiter))
           {
              return waiter;
           }


           try
           {
              var res = action(p1, p2, p3);
              return TaskEx.FromResult(res);
           }
           finally
           {
              EndWriterImpl();
           }
        }

        public Task<TOut> WriteAsync<T1, T2, T3, T4, TOut>(T1 p1,
                                                           T2 p2,
                                                           T3 p3,
                                                           T4 p4,
                                                           Func<T1, T2, T3, T4, TOut> func)
        {
           if (_isDisposed)
              return TaskEx.FromResult(default(TOut)!);

           if (TryGetWriterWaiter((w,
                                   c) => new FuncWaiter<T1, T2, T3, T4, TOut>(p1, p2,
                 p3, p4, func, w, c),
              ref _writerCount,
              out var waiter))
              return waiter;

           try
           {
              var res = func(p1, p2, p3, p4);
              return TaskEx.FromResult(res);
           }
           finally
           {
              EndWriterImpl();
           }
        }

        public Task<TResult> WriteTaskAsync<TParam1, TParam2, TResult>(TParam1 p1,
                                                                       TParam2 p2,
                                                                       Func<TParam1, TParam2, Task<TResult>> action)
        {
            if (_isDisposed)
                return TaskEx.FromResult(default(TResult)!);

            if (TryGetWriterWaiter((w,
                                    _) => new AsyncTaskAwaiter<TParam1, TParam2, TResult>(p1, p2,
                    action, w),
                ref _writerCount,
                out var waiter))
                return waiter;

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

        //public Task<TResult> WriteTaskAsync<TParam1, TParam2, TParam3, TResult>(TParam1 p1,
        //                                                               TParam2 p2,
        //                                                               TParam3 p3,
        //                                                               Func<TParam1, TParam2, TParam3, TParam4, TResult> action)
        //{
        //   if (_isDisposed)
        //      return TaskEx.FromResult(default(TResult)!);

        //   if (TryGetWriterWaiter((w,
        //                           _) => new AsyncTaskAwaiter<TParam1, TParam2, TResult>(p1, p2,
        //         action, w),
        //      ref _writerCount,
        //      out var waiter))
        //      return waiter;

        //   try
        //   {
        //      var res = action(p1, p2);
        //      return res;
        //   }
        //   finally
        //   {
        //      EndWriterImpl();
        //   }
        //}

        public Task<TResult> WriteAsync<TParam, TResult>(TParam param,
                                                         Func<TParam, Task<TResult>> action)
        {
            if (_isDisposed)
                return TaskEx.FromResult(default(TResult)!);


            if (TryGetWriterWaiter((w,
                                    _) => new AsyncTaskAwaiter<TParam, TResult>(param,
                    action, w),
                ref _writerCount,
                out var waiter))
                return waiter;

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

            if (TryGetWriterWaiter((w,
                                    _) => new AsyncTaskAwaiter<TParam, Int32>(param,
                    action, w),
                ref _writerCount,
                out var waiter))
                return waiter;

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

        /// <summary>
        ///     increments reader count if no waiter is needed!
        /// </summary>
        private Boolean TryGetWriterWaiter<TWriterWaiter>(Func<WorkerTypes, CancellationToken, TWriterWaiter> builder,
                                                          ref Int32 writerCount,
                                                          out TWriterWaiter value)
            where TWriterWaiter : ITaskWaiter
        {
            lock (_lockObj)
            {
                if (writerCount > 0 || _readerCount > 0 || _waiterCount > 0)
                {
                    _waiterCount++;
                    value = builder(WorkerTypes.Writer, _disposeClearCancellation.Token);

                    _waiters.Enqueue(value);
                    return true;
                }

                writerCount++;

                value = default!;
                return false;
            }
        }

       
    }
}
