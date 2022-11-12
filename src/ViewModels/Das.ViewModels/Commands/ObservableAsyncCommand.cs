using System;
using System.Threading.Tasks;
using Das.Views;

namespace Das.ViewModels.Commands
{
    public class ObservableAsyncCommand : ObservableCommandBase
    {
        public ObservableAsyncCommand(Func<Task> action,
                                      ISingleThreadedInvoker staInvoker)
            : base(staInvoker)
        {
            _action = action;
        }

        public override void Execute(Object parameter)
        {
            _action().ConfigureAwait(false);
            //.GetAwaiter()
            //.GetResult();
        }

        public override async Task ExecuteAsync()
        {
            await _action();
        }

        public override async Task ExecuteAsync(Object paramValue)
        {
            await _action();
        }

        private readonly Func<Task> _action;
    }
}
