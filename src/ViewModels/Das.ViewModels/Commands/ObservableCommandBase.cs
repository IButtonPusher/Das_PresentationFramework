using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Das.Views;
using Das.Views.Mvvm;

namespace Das.ViewModels.Commands
{
    public abstract class ObservableCommandBase : NotifyPropertyChangedBase,
                                                  IObservableCommand,
                                                  ICommand
    {
        protected ObservableCommandBase(ISingleThreadedInvoker staInvoker)
        {
            _staInvoker = staInvoker;
            _isExecutable = true;
        }

        public virtual Boolean CanExecute(Object parameter)
        {
            return IsExecutable;
        }

        public abstract void Execute(Object parameter);

        public event EventHandler? CanExecuteChanged;

        public abstract Task ExecuteAsync();

        public abstract Task ExecuteAsync(Object paramValue);

        public String? Description
        {
            get => _description;
            set => SetValue(ref _description, value);
        }

        public Boolean IsExecutable
        {
            get => _isExecutable;
            set => SetValue(ref _isExecutable, value, OnCanExecuteChanged);
        }

        public virtual Boolean Equals(IObservableCommand other)
        {
            return ReferenceEquals(this, other);
        }

        protected static TReturn ThrowParamException<TParam, TReturn>()
        {
            throw new InvalidOperationException(
                $"Action delegate requires a parameter of type {typeof(TParam)})");
        }

        protected virtual async Task OnCanExecuteChanged(Boolean canExecute)
        {
            var can = CanExecuteChanged;
            if (can == null)
                return;

            await _staInvoker.InvokeAsync(() => { CanExecuteChanged?.Invoke(canExecute, EventArgs.Empty); });
        }

        public override void Dispose()
        {
            base.Dispose();

            CanExecuteChanged = null;
        }

        private readonly ISingleThreadedInvoker _staInvoker;

        private String? _description;
        private Boolean _isExecutable;
    }
}
