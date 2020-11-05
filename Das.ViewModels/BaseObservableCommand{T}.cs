using System;
using System.Threading.Tasks;
using Das.Views.Mvvm;

namespace Das.ViewModels
{
    public class BaseObservableCommand<T> : NotifyPropertyChangedBase,
                                            IObservableCommand<T>
    {
        public BaseObservableCommand(Func<T, Task> action)
        {
            _action = action;
        }


        public Boolean Equals(IObservableCommand other)
        {
            throw new NotImplementedException();
        }

        public String? Description
        {
            get => _description;
            set => SetValue(ref _description, value);
        }

        public Boolean IsExecutable
        {
            get => _canExecuteVal;
            set => SetValue(ref _canExecuteVal, value);
        }

        Task IObservableCommand.ExecuteAsync()
        {
            throw new NotSupportedException();
        }

        public async Task ExecuteAsync(Object paramValue)
        {
            if (paramValue is T fine)
                await _action(fine);

            throw new InvalidCastException();
        }

        public async Task ExecuteAsync(T paramValue)
        {
            await _action(paramValue);
        }

        public Task ExecuteAsync(T[] paramValues)
        {
            throw new NotImplementedException();
        }

        private readonly Func<T, Task> _action;

        private Boolean _canExecuteVal;

        private String? _description;
    }
}