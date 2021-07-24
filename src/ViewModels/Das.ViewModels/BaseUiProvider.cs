using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Das.Views;
using Das.Views.Input;

namespace Das.ViewModels
{
    public abstract class BaseUiProvider : IUiProvider
    {
        public virtual void BeginNotify(String text)
        {
            throw new NotImplementedException();
        }

        public virtual Task<Boolean> Confirm(String message, 
                                             String title)
        {
            throw new NotImplementedException();
        }

        public virtual Task<Boolean> Confirm(String message)
        {
            throw new NotImplementedException();
        }

        public virtual IObservableCommand GetCommand(Action action)
        {
            throw new NotImplementedException();
        }

        public virtual IObservableCommand GetCommand(Action action, 
                                                     String description)
        {
            throw new NotImplementedException();
        }

        public virtual IObservableCommand GetCommand(Func<Task> action, 
                                                     String description)
        {
            throw new NotImplementedException();
        }

        public virtual IObservableCommand<T> GetCommand<T>(Func<T, Task> action)
        {
            return new BaseObservableCommand<T>(action);
        }

        public virtual IObservableCommand<T> GetCommand<T>(Action<T> action)
        {
            throw new NotImplementedException();
        }

        public virtual IObservableCommand<T> GetCommand<T>(Func<T, Task> action,
                                                           String description)
        {
            throw new NotImplementedException();
        }

        public virtual IObservableCommand<T> GetCommand<T>(Func<IEnumerable<T>, Task> action, 
                                                           String description)
        {
            throw new NotImplementedException();
        }

        public virtual IObservableCommand<T> GetCommand<T>(Func<T[], Task> action)
        {
            throw new NotImplementedException();
        }

        public virtual IObservableCommand<T> GetCommand<T>(Func<T[], Task> action, 
                                                           String description)
        {
            throw new NotImplementedException();
        }

        public virtual IObservableCommand GetCommand(Action action, 
                                                     INotifyPropertyChanged viewModel, 
                                                     String canExecuteProperty)
        {
            throw new NotImplementedException();
        }

        public virtual IObservableCommand GetCommand(Func<Task> action)
        {
            return new BaseObservableCommand(action, this);
        }

        public virtual Task HandleErrorAsync(String wasDoing, Exception ex)
        {
            throw new NotImplementedException();
        }

        public abstract void Notify(String text);

        public virtual void Notify(String text, String title)
        {
            throw new NotImplementedException();
        }

        public virtual Task NotifyAsync(String text)
        {
            throw new NotImplementedException();
        }

        public abstract Task NotifyAsync(String text,
                                         String title);

        public virtual void NotifyError(String message)
        {
            throw new NotImplementedException();
        }

        public virtual Task NotifyErrorAsync(String message)
        {
            throw new NotImplementedException();
        }

        public virtual Task SetCursor(MousePointers cursor)
        {
            throw new NotImplementedException();
        }

        public abstract void BrowseToUri(Uri uri);

        //public abstract ValueSize GetMainViewSize();

        public virtual void BeginInvoke(Action action)
        {
            throw new NotImplementedException();
        }

        public abstract void Invoke(Action action);

        public virtual void Invoke(Action action, Int32 priority)
        {
            throw new NotImplementedException();
        }

        public abstract Task InvokeAsync(Func<Task> action);

        public virtual T Invoke<T>(Func<T> action)
        {
            throw new NotImplementedException();
        }

        public virtual Task InvokeAsync(Action action)
        {
            throw new NotImplementedException();
        }

        public virtual Task<T> InvokeAsync<T>(Func<T> action)
        {
            throw new NotImplementedException();
        }

        public virtual Task<TOutput> InvokeAsync<TInput, TOutput>(TInput input, 
                                                                  Func<TInput, TOutput> action)
        {
            throw new NotImplementedException();
        }

        public virtual Task InvokeAsync<TInput>(TInput input, Func<TInput, Task> action)
        {
            throw new NotImplementedException();
        }

        public virtual Task<T> InvokeAsync<T>(Func<Task<T>> action)
        {
            throw new NotImplementedException();
        }
    }
}
