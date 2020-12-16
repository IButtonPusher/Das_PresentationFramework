using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.Styles;

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

        public Boolean OnMouseInput<TArgs>(TArgs args, 
                                           InputAction action)
            where TArgs : IMouseInputEventArgs<TArgs>
        {
            _lastInputContext = args.InputContext;
            
            //System.Diagnostics.Debug.WriteLine("Input handler received: " + args + " capturing: " + 
            //                                   _inputCapturingMouse);
            
            var isButtonAction = (InputAction.AnyMouseButton & action) > InputAction.None;
            IInteractiveView? handledBy = null;

            if (_inputCapturingMouse is IHandleInput<TArgs> captureHandler && 
                _inputCapturingMouse is IVisualElement captureElement)
            {
                OnMouseInputWithCapture(captureHandler, captureElement,
                    args, action, isButtonAction, out handledBy);

                // smile
            }

            if (handledBy == null)
            {
                OnMouseInputNoCapture(args, action, isButtonAction, out handledBy);
            }

            if (_handledMouseDown != null && (action == InputAction.LeftMouseButtonUp ||
                                              action == InputAction.RightMouseButtonUp))
                _handledMouseDown = null;

            //if (action == InputAction.MouseDrag && handledBy == null)
            //{}

            return handledBy !=null;
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private Boolean OnMouseInputWithCapture<TArgs>(IHandleInput<TArgs> capture,
                                                       IVisualElement captureAsVisual,
                                                       TArgs args,
                                                       InputAction action,
                                                       Boolean isButtonAction,
                                                       out IInteractiveView handledBy)
            where TArgs : IMouseInputEventArgs<TArgs>
        {
            if (!(_elementLocator.TryGetLastRenderBounds(captureAsVisual) is { } bounds))
            {
                handledBy = default!;
                return false;
                //todo: this is not good
            }

            if (TryHandleMouseAction(args, capture, isButtonAction, action, bounds))
            {
                handledBy = capture;
                return true;
            }

            // if the capture doesn't want it, maybe a parent visual does
            var argsOverCapture = args.Offset(bounds.TopLeft);
            return OnMouseInputNoCapture(argsOverCapture, action, isButtonAction, out handledBy);
        }

        private Boolean OnMouseInputNoCapture<TArgs>(TArgs args,
                                                     InputAction action,
                                                     Boolean isButtonAction,
                                                     out IInteractiveView handledBy)
            where TArgs : IMouseInputEventArgs<TArgs>
        {
            foreach (var clickable in _elementLocator.GetRenderedVisualsForMouseInput<TArgs, IPoint2D>(
                args.Position, action))
            {
                var element = clickable.Element;

                if (element == _inputCapturingMouse)
                    continue;

                if (TryHandleMouseAction(args, element, isButtonAction, action,
                    clickable.Position))
                {
                    handledBy = element;
                    return true;
                }
            }

            handledBy = default!;
            return false;
        }

        private Boolean TryHandleMouseAction<TArgs>(TArgs args,
                                                    IHandleInput<TArgs> element,
                                                    Boolean isButtonAction,
                                                    InputAction action,
                                                    ICube offsetCube)
            where TArgs : IMouseInputEventArgs<TArgs>
        {
            var margs = args.Offset(offsetCube.TopLeft);

            if (isButtonAction)
            {
                return HandleButtonAction(margs, element, action);

            }

            return element.OnInput(margs);
        }

        private Boolean HandleButtonAction<TArgs>(TArgs margs,
                                                            IHandleInput<TArgs> element,
                                                            InputAction action)
            where TArgs : IMouseInputEventArgs<TArgs>//, IMouseButtonEventArgs
        {
            var handledActionDirectly = element.OnInput(margs);

            switch (action)
            {
                case InputAction.LeftMouseButtonDown:
                    if ((handledActionDirectly ||
                        IsHandlesAction(element, InputAction.LeftClick)) && 
                        element is IHandleInput<MouseDownEventArgs> leftDowner)
                    {
                        _handledMouseDown = leftDowner;
                        if (element.CurrentStyleSelector.Contains(StyleSelector.Active))
                            _mouseActiveElement = leftDowner;
                    }

                    break;
                case InputAction.RightMouseButtonDown:
                    if ((handledActionDirectly ||
                        IsHandlesAction(element, InputAction.RightClick)) && 
                        element is IHandleInput<MouseDownEventArgs> rightDowner)
                    {
                        _handledMouseDown = rightDowner;
                    }

                    break;
                
                case InputAction.LeftMouseButtonUp:
                    if (ReferenceEquals(_handledMouseDown, element) &&
                        IsHandlesAction(element, InputAction.LeftClick) && 
                        element is IHandleInput<MouseClickEventArgs> clickMe)
                    {
                        if (!handledActionDirectly &&
                            IsHandlesAction(element, InputAction.LeftMouseButtonUp))
                        {
                            //handles mouse up but didn't handle this one so don't give him the click
                            break;
                        }
                        
                        

                        var clickArgs = new MouseClickEventArgs(margs.Position,
                            MouseButtons.Left, 1, margs.InputContext);
                        if (clickMe.OnInput(clickArgs))
                            handledActionDirectly = true;
                    }

                    break;
                case InputAction.RightMouseButtonUp:
                    if (ReferenceEquals(_handledMouseDown, element) &&
                        IsHandlesAction(element, InputAction.RightClick) && 
                        element is IHandleInput<MouseClickEventArgs> rightClickMe)
                    {
                        var clickArgs = new MouseClickEventArgs(margs.Position,
                            MouseButtons.Right, 1, margs.InputContext);
                        if (rightClickMe.OnInput(clickArgs))
                            handledActionDirectly = true;
                    }
                    break;


                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }

            return handledActionDirectly;
        }

        private static Boolean IsHandlesAction(IInteractiveView view,
                                               InputAction action)
        {
            return (view.HandlesActions & action) > InputAction.None;
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

            foreach (var visual in _elementLocator.GetRenderedVisualsForMouseInput<MouseOverEventArgs, TPoint>(
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

            foreach (var visual in _elementLocator.GetRenderedVisualsForMouseInput<MouseOverEventArgs, TPoint>(
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
            if (ReferenceEquals(_inputCapturingMouse, view))
                return true;

            // ReSharper disable once SuspiciousTypeConversion.Global
            if (_inputCapturingMouse is IHandleInput<LostCaptureEventArgs> lost &&
                _lastInputContext is { })
                lost.OnInput(new LostCaptureEventArgs(_lastInputContext));
            
            //System.Diagnostics.Debug.WriteLine(view + " wants to capture the mouse. Current capture:" +
            //                                   _inputCapturingMouse );
            
            //if (_inputCapturingMouse != null && _inputCapturingMouse != view)
            //    return false;

            _inputCapturingMouse = view;
            if (view is IHandleInput<MouseDownEventArgs> downer)
                _handledMouseDown = downer;
            return true;
        }

        public Boolean TryReleaseMouseCapture(IVisualElement view)
        {
            if (_inputCapturingMouse != view)
                return false;

            _inputCapturingMouse = null;
            return true;
        }

        public IVisualElement? GetVisualWithMouseCapture()
        {
            return _inputCapturingMouse;
        }

        public void OnKeyboardStateChanged()
        {
            //TODO_IMPLEMENT_ME();
        }

        private readonly IElementLocator _elementLocator;

        //private readonly Double _offsetMultiplier;
        //private readonly Boolean _isOffsetPositions;

        //private static InputAction 
        private IHandleInput<MouseDownEventArgs>? _handledMouseDown;
        private IVisualElement? _inputCapturingMouse;
        private IHandleInput<MouseDownEventArgs>? _mouseActiveElement;
        private IHandleInput<MouseOverEventArgs>? _lastMouseOverVisual;
        private IInputContext? _lastInputContext;
    }
}