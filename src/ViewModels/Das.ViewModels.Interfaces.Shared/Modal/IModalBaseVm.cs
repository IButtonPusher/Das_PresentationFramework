using System;
using System.Threading.Tasks;

#if NET40
using ICommand = System.Object;
#else
using System.Windows.Input;
#endif

namespace Das.ViewModels
{
    public interface IModalBaseVm
    {
        ICommand CancelCommand { get; }

        ICommand OkCommand { get; }

        Boolean CanClose { get; }

        Boolean CanMinimize { get; }

        Boolean CanMaximize { get; }

        Boolean CanResize { get; }
    }
}
