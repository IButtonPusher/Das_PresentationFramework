using System;
using Das.ViewModels;
using Das.Views;

namespace ViewModels.Shared
{

    public abstract class InvokableViewModel : BaseViewModel
    {
        protected readonly ISingleThreadedInvoker StaInvoker;

        public InvokableViewModel(ISingleThreadedInvoker invoker)
        {
            StaInvoker = invoker;
        }
    }
}
