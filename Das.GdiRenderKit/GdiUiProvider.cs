using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Das.ViewModels;
using Das.Views.Input;

namespace Das.Views.Gdi
{
    public class GdiUiProvider : BaseUiProvider,
                                 IUiProvider
    {
        public void BeginInvoke(Action action)
        {
            throw new NotImplementedException();
        }

        public void Invoke(Action action)
        {
            throw new NotImplementedException();
        }

        public void Invoke(Action action, Int32 priority)
        {
            throw new NotImplementedException();
        }

        public T Invoke<T>(Func<T> action)
        {
            throw new NotImplementedException();
        }

        public Task InvokeAsync(Action action)
        {
            throw new NotImplementedException();
        }

        public Task<T> InvokeAsync<T>(Func<T> action)
        {
            throw new NotImplementedException();
        }

        public Task<TOutput> InvokeAsync<TInput, TOutput>(TInput input, Func<TInput, TOutput> action)
        {
            throw new NotImplementedException();
        }

        public Task InvokeAsync<TInput>(TInput input, Func<TInput, Task> action)
        {
            throw new NotImplementedException();
        }

        public Task<T> InvokeAsync<T>(Func<Task<T>> action)
        {
            throw new NotImplementedException();
        }

        public void BeginNotify(String text)
        {
            throw new NotImplementedException();
        }

        public Task<Boolean> Confirm(String message, String title)
        {
            throw new NotImplementedException();
        }

        public Task<Boolean> Confirm(String message)
        {
            throw new NotImplementedException();
        }

        public IObservableCommand GetCommand(Action action)
        {
            throw new NotImplementedException();
        }

        public IObservableCommand GetCommand(Action action, String description)
        {
            throw new NotImplementedException();
        }

        public IObservableCommand GetCommand(Func<Task> action, String description)
        {
            throw new NotImplementedException();
        }


        public IObservableCommand<T> GetCommand<T>(Action<T> action)
        {
            throw new NotImplementedException();
        }

        public IObservableCommand<T> GetCommand<T>(Func<T, Task> action, String description)
        {
            throw new NotImplementedException();
        }

        public IObservableCommand<T> GetCommand<T>(Func<IEnumerable<T>, Task> action, String description)
        {
            throw new NotImplementedException();
        }

        public IObservableCommand<T> GetCommand<T>(Func<T[], Task> action)
        {
            throw new NotImplementedException();
        }

        public IObservableCommand<T> GetCommand<T>(Func<T[], Task> action, String description)
        {
            throw new NotImplementedException();
        }

        public IObservableCommand GetCommand(Action action, INotifyPropertyChanged viewModel, String canExecuteProperty)
        {
            throw new NotImplementedException();
        }

        public IObservableCommand GetCommand(Func<Task> action)
        {
            throw new NotImplementedException();
        }

        public Task HandleErrorAsync(String wasDoing, Exception ex)
        {
            throw new NotImplementedException();
        }

        public void Notify(String text, String title)
        {
            throw new NotImplementedException();
        }

        public Task NotifyAsync(String text)
        {
            throw new NotImplementedException();
        }

        public void NotifyError(String message)
        {
            throw new NotImplementedException();
        }

        public Task NotifyErrorAsync(String message)
        {
            throw new NotImplementedException();
        }

        public Task SetCursor(MousePointers cursor)
        {
            throw new NotImplementedException();
        }

        public Task<IAsyncDisposable> WithCursor(MousePointers cursor)
        {
            throw new NotImplementedException();
        }
    }
}
