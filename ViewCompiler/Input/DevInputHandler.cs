using System;
using System.Linq;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Views.Core.Geometry;
using Das.Views.Input;
using Das.Views.Rendering;
using ViewCompiler;

namespace Das.Views.DevKit
{
    public class DevInputHandler : IInputHandler
    {
        public void Dispose()
        {
            
        }

        public IRenderKit RenderKit { get; set; }

        public IInputProvider InputProvider { get; set; }

        public Boolean OnMouseHovering(MouseDownEventArgs args)
        {
            return false;
        }

        public Boolean OnMouseUp(MouseUpEventArgs args)
        {
            return false;
        }

        public Boolean OnMouseInput<TArgs>(TArgs args, 
                                        InputAction action) 
            where TArgs : IMouseInputEventArgs<TArgs>
        {
            if (action == InputAction.LeftMouseButtonDown &&
                args is MouseDownEventArgs e)
            {
                OnMouseDown(e);
                return true;
            }

            return false;
        }

        public Boolean OnMouseMove<TPoint>(TPoint position, 
                                           IInputContext inputContext) 
            where TPoint : IPoint2D
        {
            return false;
        }

        public Boolean TryCaptureMouseInput(IVisualElement view)
        {
            return false;
        }

        public Boolean TryReleaseMouseCapture(IVisualElement view)
        {
            return false;
        }

        public Boolean OnMouseDown(MouseDownEventArgs args)
        {
            _lastPosition = args.Position;
            UpdateSelectedItems(args.Position);
            return true;
        }


        //public Boolean OnMouseUp(MouseDownEventArgs args) => false;

        public void OnKeyboardStateChanged()
        {
            //if (!AreButtonsPressed(KeyboardButtons.Control,
            //    KeyboardButtons.Shift))
                return;

            if (_lastPosition is {} position)
                UpdateSelectedItems(position);
        }

        private void UpdateSelectedItems(IPoint2D position)
        {
            var cursor = position;
            var elements = _elementLocator.GetElementsAt(cursor).ToArray();

            var newArr = elements.ToArray();
            if (newArr.Congruent(_designViewUpdater.SelectedVisuals))
                return;

            _designViewUpdater.SelectedVisuals = newArr;
        }

        private readonly IElementLocator _elementLocator;
        private readonly DesignViewUpdater _designViewUpdater;
        private IPoint2D? _lastPosition;

        public DevInputHandler(IElementLocator elementLocator, DesignViewUpdater designViewUpdater)
        {
            _elementLocator = elementLocator;
            _designViewUpdater = designViewUpdater;
        }
    }
}
