using System;
using System.Threading.Tasks;
using Das.ViewModels;
using Das.Views.DataBinding;

namespace Das.Views.Controls
{
    public interface IButtonBase : IBindableElement,
                                   IInteractiveView
    {
        IObservableCommand? Command { get; }
    }
}