using Das.Views.Input;
using Das.ViewsModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Das.Views
{
    public interface IUiProvider : ISingleThreadedInvoker
    {
        Task SetCursor(MousePointers cursor);

        Task<IAsyncDisposable> WithCursor(MousePointers cursor);

        void NotifyError(String message);

        Task NotifyErrorAsync(String message);

        //void HandleError(String wasDoing, Exception ex);

        Task HandleErrorAsync(String wasDoing, Exception ex);

        IObservableCommand GetCommand(Action action);

        IObservableCommand GetCommand(Action action, String description);

        IObservableCommand GetCommand(Func<Task> action, String description);

        //IObservableCommand GetCommand(Action<Object> action);

        //IObservableCommand GetCommand(Func<Object, Task> action);

        IObservableCommand<T> GetCommand<T>(Func<T, Task> action);

        IObservableCommand<T> GetCommand<T>(Action<T> action);

        IObservableCommand<T> GetCommand<T>(Func<T, Task> action,
            String description);

        IObservableCommand<T> GetCommand<T>(Func<IEnumerable<T>, Task> action,
            String description);

        IObservableCommand<T> GetCommand<T>(Func<T[], Task> action);

        IObservableCommand<T> GetCommand<T>(Func<T[], Task> action, String description);

        IObservableCommand GetCommand(Action action,
            INotifyPropertyChanged viewModel, String canExecuteProperty);

        Task<Boolean> Confirm(String message, String title);

        Task<Boolean> Confirm(String message);

        IObservableCommand GetCommand(Func<Task> action);


        void BeginNotify(String text);

        Task NotifyAsync(String text);

        void Notify(String text, String title);
    }
}
