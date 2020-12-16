using System;
using System.Threading.Tasks;
using Das.Views;

namespace Das.ViewModels
{
    public abstract class InvokableViewModel : BaseViewModel
    {
        protected InvokableViewModel(ISingleThreadedInvoker invoker)
        {
            StaInvoker = invoker;
        }

        protected readonly ISingleThreadedInvoker StaInvoker;
    }
}