using System;
using System.Threading.Tasks;
using Das.Views;

namespace Das.ViewModels.Commands
{
    public class ObservableActionCommand : ObservableCommandBase
    {
        public ObservableActionCommand(Action action,
                                       ISingleThreadedInvoker staInvoker) : base(staInvoker)
        {
            _action = action;
        }

        public override void Execute(Object parameter)
        {
            _action();
        }

        public override Task ExecuteAsync()
        {
            _action();
            return Task.CompletedTask;
        }

        public override Task ExecuteAsync(Object paramValue)
        {
            _action();
            return Task.CompletedTask;
        }

        private readonly Action _action;
    }
}
