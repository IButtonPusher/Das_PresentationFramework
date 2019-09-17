using System;
using Das.Views.Core.Geometry;
using Das.Views.Core.Input;

namespace Das.Views.Input
{
    public interface IInputHandler : IDisposable
    {
        IRenderKit RenderKit { get; set; }

        IInputProvider InputProvider { get; set; }

        void OnMouseHovering(IPoint position);

        void OnMouseDown(MouseButtons button, IPoint position);

        void OnMouseUp(MouseButtons button, IPoint position);

        void OnKeyboardStateChanged();
    }
}