using System;
using System.Threading.Tasks;
using Das.ViewModels;
using Das.Views;

namespace ViewModels.Shared
{
    public abstract class InvokableViewModel : BaseViewModel
    {
        public InvokableViewModel(ISingleThreadedInvoker invoker)
        {
            StaInvoker = invoker;
        }

        protected readonly ISingleThreadedInvoker StaInvoker;
    }
}