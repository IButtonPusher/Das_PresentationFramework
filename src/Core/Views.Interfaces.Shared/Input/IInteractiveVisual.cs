using System;
using System.Threading.Tasks;
using Das.Views.Input;

namespace Das.Views;

public interface IInteractiveVisual : IVisualElement,
                                      IHandleInput
{
   //InputAction HandlesActions { get; }

   Boolean IsActive { get; set; }

   Boolean IsFocused { get; }

   Boolean IsMouseOver { get; }
}