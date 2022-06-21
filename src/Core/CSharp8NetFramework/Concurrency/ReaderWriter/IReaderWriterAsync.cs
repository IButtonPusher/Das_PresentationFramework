using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace System.Threading;

public interface IReaderWriterAsync : IDisposable
{
    void Clear();

    T Read<T>(Func<T> action);

    IDisposable Read();

    TResult Read<TInput, TResult>(TInput input,
                                  Func<TInput, TResult> action);

    TResult Read<TInput1, TInput2, TResult>(TInput1 input,
                                            TInput2 input2,
                                            Func<TInput1, TInput2, TResult> action);

    Task<TResult> ReadAsync<TResult>(Func<TResult> action);

    Task<TResult> ReadAsync<TParam1, TResult>(TParam1 p1,
                                              Func<TParam1, TResult> action);

    Task<TResult> ReadTaskAsync<TParam1, TResult>(TParam1 p1,
                                                  Func<TParam1,
                                                      Task<TResult>> action);

    Task<TResult> ReadAsync<TParam1, TParam2, TResult>(TParam1 p1,
                                                       TParam2 p2,
                                                       Func<TParam1, TParam2, TResult> action);

    Task<TResult> ReadAsync<TParam1, TParam2, TParam3, TResult>(TParam1 p1,
                                                                TParam2 p2,
                                                                TParam3 p3,
                                                                Func<TParam1, TParam2, TParam3, TResult> action);

    IEnumerable<T> ReadMany<T>(Func<IEnumerable<T>> action);

    IEnumerable<TResult> ReadMany<TParam, TResult>(TParam p1,
                                                   Func<TParam, IEnumerable<TResult>> action);

    IAsyncEnumerable<TResult> ReadManyAsync<TResult>(Func<IEnumerable<TResult>> action);

    IAsyncEnumerable<TResult> ReadManyAsync<TParam1, TResult>(Func<TParam1, IEnumerable<TResult>> action,
                                                              TParam1 p1);

    IAsyncEnumerable<TResult> ReadManyAsync<TParam1, TParam2, TParam3, TResult>(
        Func<TParam1, TParam2, TParam3, IEnumerable<TResult>> action,
        TParam1 p1,
        TParam2 p2,
        TParam3 p3);


    IAsyncEnumerable<TResult> ReadManyAsync<TParam1, TParam2, TResult>(
        Func<TParam1, TParam2, IEnumerable<TResult>> action,
        TParam1 p1,
        TParam2 p2);

    IAsyncEnumerable<TResult> ReadManyAsync<TParam, TResult>(TParam p1,
                                                             Func<TParam, IAsyncEnumerable<TResult>> action);

    Int32 Write(Func<Int32> action);

    TResult Write<TParam1, TResult>(TParam1 p1,
                                    Func<TParam1, TResult> action);

    Int32 Write<TParam1, TParam2>(TParam1 p1,
                                  TParam2 p2,
                                  Func<TParam1, TParam2, Int32> action);

    void Write<TParam1, TParam2>(TParam1 p1,
                                 TParam2 p2,
                                 Action<TParam1, TParam2> action);

    void Write<TParam1, TParam2, TParam3>(TParam1 p1,
                                          TParam2 p2,
                                          TParam3 p3,
                                          Action<TParam1, TParam2, TParam3> action);

    void Write(Action action);

    Task WriteTaskAsync(Func<Task> asyncAction);

    Task<Int32> WriteAsync(Func<Int32> action);

    Task<TResult> WriteAsync<TParam, TResult>(TParam param,
                                              Func<TParam, TResult> action);


    Task<TResult> WriteAsync<TParam, TResult>(TParam param,
                                              Func<TParam, Task<TResult>> action);

    Task<TResult> WriteAsync<TParam1, TParam2, TResult>(TParam1 p1,
                                                        TParam2 p2,
                                                        Func<TParam1, TParam2, TResult> action);

    Task WriteAsync<TParam1, TParam2>(TParam1 p1,
                                      TParam2 p2,
                                      Func<TParam1, TParam2, Task> action);

    IAsyncEnumerable<TResult> WriteAsync<TParam1, TParam2, TResult>(TParam1 p1,
                                                           TParam2 p2,
                                                           RunManyAsync<TParam1, TParam2, TResult> action);

    IAsyncEnumerable<TResult> WriteManyAsync<TParam1, TParam2, TResult>(TParam1 p1,
                                                                    TParam2 p2,
                                                                    RunMany<TParam1, TParam2, TResult> action);

    Task<TResult> WriteTaskAsync<TParam1, TParam2, TResult>(TParam1 p1,
                                                            TParam2 p2,
                                                            Func<TParam1, TParam2, Task<TResult>> action);

    Task WriteAsync<TParam1>(TParam1 p1,
                             Action<TParam1> action);

    Task WriteAsync<TParam1, TParam2>(TParam1 p1,
                                      TParam2 p2,
                                      Action<TParam1, TParam2> action);

    Task WriteAsync<TParam1, TParam2, TParam3>(TParam1 p1,
                                               TParam2 p2,
                                               TParam3 p3,
                                               Action<TParam1, TParam2, TParam3> action);

    Task WriteTaskAsync<TParam1, TParam2, TParam3, TParam4>(TParam1 p1,
                                                            TParam2 p2,
                                                            TParam3 p3,
                                                            TParam4 p4,
                                                            Func<TParam1, TParam2, TParam3, TParam4, Task> action);

    Task<Int32> WriteAsync<TParam>(TParam param,
                                   Func<TParam, Task<Int32>> action);
}

public delegate IAsyncEnumerable<TResult> RunManyAsync<in TParam1, in TParam2, out TResult>(TParam1 p1, 
                                                                                         TParam2 items);

public delegate IEnumerable<TResult> RunMany<in TParam1, in TParam2, out TResult>(TParam1 p1,
    TParam2 items);

    