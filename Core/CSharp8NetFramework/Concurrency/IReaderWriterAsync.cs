using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// ReSharper disable UnusedMember.Global

namespace System.Threading
{
    public interface IReaderWriterAsync : IDisposable
    {
        void Clear();

        T Read<T>(Func<T> action);

        IDisposable Read();

        TResult Read<TInput, TResult>(TInput input,
                                      Func<TInput, TResult> action);

        Task<TResult> ReadAsync<TResult>(Func<TResult> action);

        Task<TResult> ReadAsync<TParam1, TResult>(TParam1 p1,
                                                  Func<TParam1, TResult> action);

        Task<TResult> ReadAsync<TParam1, TResult>(TParam1 p1,
                                                  Func<TParam1,
                                                      Task<TResult>> action);

        Task<TResult> ReadAsync<TParam1, TParam2, TResult>(TParam1 p1, TParam2 p2,
                                                           Func<TParam1, TParam2, TResult> action);

        IEnumerable<T> ReadMany<T>(Func<IEnumerable<T>> action);

        IEnumerable<TResult> ReadMany<TParam, TResult>(TParam p1,
                                                       Func<TParam, IEnumerable<TResult>> action);

        IAsyncEnumerable<TResult> ReadManyAsync<TResult>(Func<IEnumerable<TResult>> action);

        IAsyncEnumerable<TResult> ReadManyAsync<TParam, TResult>(TParam p1,
                                                                 Func<TParam, IAsyncEnumerable<TResult>> action);

        Int32 Write(Func<Int32> action);

        Int32 Write<TParam1>(TParam1 p1,
                             Func<TParam1, Int32> action);

        Int32 Write<TParam1, TParam2>(TParam1 p1,
                                      TParam2 p2,
                                      Func<TParam1, TParam2, Int32> action);

        void Write(Action action);

        Task WriteAsync(Func<Task> asyncAction);

        Task<Int32> WriteAsync(Func<Int32> action);

        Task<Int32> WriteAsync<TParam>(TParam param,
                                       Func<TParam, Int32> action);

        Task<Int32> WriteAsync<TParam>(TParam param,
                                       Func<TParam, Task<Int32>> action);
    }
}