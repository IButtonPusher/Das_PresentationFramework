using System;
using System.Collections.Generic;
using System.Text;
using Das.Views.Core.Geometry;
using Das.Views.Core.Input;

namespace Das.Views.Input
{
    public class BaseInputHandler : IInputHandler
    {
        public void Dispose()
        {
            //TODO_IMPLEMENT_ME();
        }

        public IRenderKit RenderKit { get; set; }

        public IInputProvider InputProvider { get; set; }

        public void OnMouseHovering(IPoint position)
        {
            //TODO_IMPLEMENT_ME();
        }

        public void OnMouseDown(MouseButtons button, IPoint position)
        {
            var clickable = _elementLocator.GetVisualForInput(position, InputAction.MouseDown);
            //TODO_IMPLEMENT_ME();
        }

        public void OnMouseUp(MouseButtons button, IPoint position)
        {
            //TODO_IMPLEMENT_ME();
        }

        public void OnKeyboardStateChanged()
        {
            //TODO_IMPLEMENT_ME();
        }

        private readonly IElementLocator _elementLocator;

        public BaseInputHandler(IElementLocator elementLocator)
        {
            _elementLocator = elementLocator;
        }
    }
}
