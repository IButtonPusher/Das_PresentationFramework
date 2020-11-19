using System;
using System.Threading.Tasks;
using Das.Views.DataBinding;
using Das.Views.Rendering;

namespace Das.Views.Controls
{
    public interface IButtonBase : IBindableElement,
                                   IInteractiveView
    {
    }
}