using System;
using System.Threading.Tasks;

namespace Das.Views
{
    public interface ISingleThreadedInvoker
    {
        void BeginInvoke(Action action);

        void Invoke(Action action);

        void Invoke(Action action, Int32 priority);

        T Invoke<T>(Func<T> action);

        Task InvokeAsync(Action action);

        Task<T> InvokeAsync<T>(Func<T> action);

        Task<TOutput> InvokeAsync<TInput, TOutput>(TInput input,
                                                   Func<TInput, TOutput> action);

        Task InvokeAsync<TInput>(TInput input,
                                 Func<TInput, Task> action);

        Task<T> InvokeAsync<T>(Func<Task<T>> action);
    }
}