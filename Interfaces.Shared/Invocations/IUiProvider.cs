using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Das.ViewModels;
using Das.Views.Core.Geometry;
using Das.Views.Input;

namespace Das.Views
{
    public interface IUiProvider : ISingleThreadedInvoker
    {
        void BeginNotify(String text);

        void BrowseToUri(Uri uri);

        Task<Boolean> Confirm(String message, String title);

        Task<Boolean> Confirm(String message);

        IObservableCommand GetCommand(Action action);

        IObservableCommand GetCommand(Action action, String description);

        IObservableCommand GetCommand(Func<Task> action, String description);


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

        IObservableCommand GetCommand(Func<Task> action);

        //ValueSize GetMainViewSize();

        Task HandleErrorAsync(String wasDoing, Exception ex);

        void Notify(String text, String title);

        Task NotifyAsync(String text);

        void NotifyError(String message);

        Task NotifyErrorAsync(String message);

        Task SetCursor(MousePointers cursor);
    }
}