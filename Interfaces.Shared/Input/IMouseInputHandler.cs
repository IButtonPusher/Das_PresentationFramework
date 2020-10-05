using System;
using Das.Views.Core.Geometry;
using Das.Views.Core.Input;

namespace Das.Views.Input
{
    public interface IMouseInputHandler
    {
        Boolean OnMouseDown(MouseDownEventArgs args);

        Boolean OnMouseHovering(MouseDownEventArgs args);

        Boolean OnMouseUp(MouseUpEventArgs args);

        void OnMouseInput<TArgs>(TArgs args, 
                                 InputAction action) 
            where TArgs : IMouseInputEventArgs;
    }
}
