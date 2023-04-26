using System;
using System.Threading.Tasks;
using Das.ViewModels;
using Das.Views.DataBinding;
using Das.Views.Input;
using Das.Views.Primitives;

namespace Das.Views.Controls;

public interface IButtonBase : IBindableElement,
                               IInteractiveVisual,
                               IInputVisual<InputVisualType>,
                               IFontVisual
{
   IObservableCommand? Command { get; }
}