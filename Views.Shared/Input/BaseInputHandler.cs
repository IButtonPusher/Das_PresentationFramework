using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;

namespace Das.Views.Input
{
    public class BaseInputHandler : IInputHandler
    {
        public BaseInputHandler(IElementLocator elementLocator)
        {
            _elementLocator = elementLocator;
        }

        public void Dispose()
        {
            //TODO_IMPLEMENT_ME();
        }


        //public Boolean OnMouseHovering(MouseDownEventArgs args)
        //{
        //    //foreach (var clickable in _elementLocator.GetVisualsForInput<IMouseInputHandler>(
        //    //    args.Position, InputAction.MouseDown))
        //    //    if (clickable.OnMouseHovering(args))
        //    //        return true;

        //    return false;
        //}

        //public Boolean OnMouseDown(MouseDownEventArgs args)
        //{
        //    foreach (var clickable in _elementLocator.GetVisualsForMouseInput<MouseDownEventArgs>(
        //        args.Position, InputAction.MouseDown))
        //        if (clickable.OnInput(args))
        //            return true;

        //    return false;
        //}

        //public Boolean OnMouseUp(MouseUpEventArgs args)
        //{
        //    foreach (var clickable in _elementLocator.GetVisualsForMouseInput<MouseUpEventArgs>(
        //        args.Position, InputAction.MouseDown))
        //        if (clickable.OnInput(args))
        //            return true;

        //    return false;
        //}

        public Boolean OnMouseInput<TArgs>(TArgs args, InputAction action)
            where TArgs : IMouseInputEventArgs<TArgs>
        {
            if (_inputCapturingMouse is IHandleInput<TArgs> capture &&
                _elementLocator.TryGetElementBounds(_inputCapturingMouse) is {} bounds)
            {
                var margs = args.Offset(bounds.TopLeft);
                if (capture.OnInput(margs))
                    return true;
            }

            foreach (var clickable in _elementLocator.GetRenderedVisualsForMouseInput<TArgs>(
                args.Position, action))
            {
                if (clickable.Element == _inputCapturingMouse)
                    continue;

                var margs = args.Offset(clickable.Position.TopLeft);

                if (clickable.Element.OnInput(margs))
                    return true;
            }

            return false;
        }

        public Boolean OnMouseMove<TPoint>(TPoint position,
                                           IInputContext inputContext)
            where TPoint : IPoint2D
        {
            switch (_inputCapturingMouse)
            {
                case IHandleInput<MouseOverEventArgs> iCare:
                    return OnMouseMoveWithCapture(position, inputContext, iCare);


                case { } _:
                    // visual has capture but doesn't care about mouse over
                    return false;

                case null:
                    return OnMouseMoveNoCapture(position, inputContext);
            }
        }

        private Boolean OnMouseMoveWithCapture<TPoint>(TPoint position,
                                                       IInputContext inputContext,
                                                       IHandleInput<MouseOverEventArgs> capturingVisual)
            where TPoint : IPoint2D
        {
            var wasOverCaptured = false;
            MouseOverEventArgs args;

            foreach (var visual in _elementLocator.GetRenderedVisualsForMouseInput<MouseOverEventArgs>(
                position, InputAction.MouseOver))
            {
                if (visual.Element != capturingVisual)
                    continue;

                wasOverCaptured = true;

                if (_lastMouseOverVisual != capturingVisual)
                {
                    // mouse wasn't over capture, now it is
                    _lastMouseOverVisual = capturingVisual;
                    args = new MouseOverEventArgs(
                        position.Offset(visual.Position.TopLeft), true, inputContext);
                    return visual.Element.OnInput(args);
                }

            }

            if (wasOverCaptured || _lastMouseOverVisual != capturingVisual)
                return false;

            // mouse used to be over capture, now it's not
            args = new MouseOverEventArgs(
                Point2D.Empty, false, inputContext);
            return capturingVisual.OnInput(args);
        }

        private Boolean OnMouseMoveNoCapture<TPoint>(TPoint position,
                                                     IInputContext inputContext)
            where TPoint : IPoint2D
        {
            MouseOverEventArgs args;
            IHandleInput<MouseOverEventArgs>? mouseNoLongerOver = null;
            var wasOverHandled = false;

            foreach (var visual in _elementLocator.GetRenderedVisualsForMouseInput<MouseOverEventArgs>(
                position, InputAction.MouseOver))
            {
                if (_lastMouseOverVisual == visual.Element)
                {
                    // was already over this one
                    return false;
                }

                mouseNoLongerOver ??= _lastMouseOverVisual;

                // mouse wasn't over capture, now it is
                _lastMouseOverVisual = visual.Element;
                args = new MouseOverEventArgs(
                    position.Offset(visual.Position.TopLeft), true, inputContext);

                wasOverHandled = true;
                var res = visual.Element.OnInput(args);

                if (mouseNoLongerOver == null)
                    return res;

                break;
            }

            if (mouseNoLongerOver != null)
            {
                // mouse was over some visual now it's over a different one
                args = new MouseOverEventArgs(
                    Point2D.Empty, false, inputContext);
                return mouseNoLongerOver.OnInput(args);
            }

            if (!wasOverHandled && _lastMouseOverVisual is {} valid)
            {
                // mouse was over some visual now it's not over anyone who cares
                args = new MouseOverEventArgs(
                    Point2D.Empty, false, inputContext);
                _lastMouseOverVisual = null;
                return valid.OnInput(args);
            }

            return false;

            
        }

        public Boolean TryCaptureMouseInput(IVisualElement view)
        {
            if (_inputCapturingMouse != null && _inputCapturingMouse != view)
                return false;

            _inputCapturingMouse = view;
            return true;
        }

        public Boolean TryReleaseMouseCapture(IVisualElement view)
        {
            if (_inputCapturingMouse != view)
                return false;

            _inputCapturingMouse = null;
            return true;
        }

        public void OnKeyboardStateChanged()
        {
            //TODO_IMPLEMENT_ME();
        }

        private readonly IElementLocator _elementLocator;
        private IVisualElement? _inputCapturingMouse;
        private IHandleInput<MouseOverEventArgs>? _lastMouseOverVisual;
    }
}