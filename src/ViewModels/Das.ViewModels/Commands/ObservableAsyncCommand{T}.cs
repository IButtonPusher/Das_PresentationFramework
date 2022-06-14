using System;
using System.Threading.Tasks;
using Das.Views;

namespace Das.ViewModels.Commands
{
    public class ObservableAsyncCommand<T> : ObservableCommandBase,
                                             IObservableCommand<T>
    {
        public ObservableAsyncCommand(Func<T, Task> action,
                                      ISingleThreadedInvoker staInvoker)
            : base(staInvoker)
        {
            _action = action;
        }

        public override Task ExecuteAsync()
        {
            return ThrowParamException<T, Task>();
        }

        public override async Task ExecuteAsync(Object paramValue)
        {
            if (paramValue is T good)
                await _action(good);
            else
                await ThrowParamException<T, Task>();
        }

        public async Task ExecuteAsync(T paramValue)
        {
            await _action(paramValue);
        }

        public async Task ExecuteAsync(T[] paramValues)
        {
            foreach (var paramValue in paramValues)
                await _action(paramValue);
        }

        public override void Execute(Object parameter)
        {
            if (parameter is T good)
                _action(good)
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
            else ThrowParamException<T, Task>();
        }

        private readonly Func<T, Task> _action;
    }
}
