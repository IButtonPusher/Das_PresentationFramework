using System;
using System.Threading.Tasks;
using Das.Views;
using Das.Views.Core.Geometry;
using Das.Views.Core.Input;
using Das.Views.Input;
using Das.Views.Rendering;
using Das.Views.Windows;

namespace Windows.Shared.Input
{
    /// <summary>
    ///     Hooks windows messages to route input for any supported windows based application (UPF native, windows forms, WPF)
    /// </summary>
    public abstract class Win32InputContext : IInputContext
    {
        public Win32InputContext(IPositionOffseter offsetter,
                                 IInputHandler inputHandler,
                                 IntPtr windowHandle)
        {
            _offsetter = offsetter;
            _inputHandler = inputHandler;
            _windowHandle = windowHandle;
        }

        public Boolean IsButtonPressed(KeyboardButtons keyboardButton)
        {
            switch (keyboardButton)
            {
                case KeyboardButtons.Control:
                    return IsButtonPressed(KeyboardButtons.LControlKey) ||
                           IsButtonPressed(KeyboardButtons.RControlKey);
                case KeyboardButtons.Shift:
                    return IsButtonPressed(KeyboardButtons.LShiftKey) ||
                           IsButtonPressed(KeyboardButtons.RShiftKey);
                default:
                    var retVal = Native.GetKeyState((Int32) keyboardButton);
                    return (retVal & 0x8000) == 0x8000;
            }
        }

        public Boolean AreButtonsPressed(KeyboardButtons button1, 
                                         KeyboardButtons button2)
        {
            return IsButtonPressed(button1) && IsButtonPressed(button2);
        }

        public Boolean AreButtonsPressed(KeyboardButtons button1, KeyboardButtons button2,
                                         KeyboardButtons button3)
        {
            return AreButtonsPressed(
                button1, button2) && IsButtonPressed(button3);
        }

        public Boolean IsButtonPressed(MouseButtons mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButtons.Left:
                    return Native.GetAsyncKeyState(Native.VK_LBUTTON) < 0;

                case MouseButtons.Right:
                    return Native.GetAsyncKeyState(Native.VK_RBUTTON) < 0;

                default:
                    throw new NotImplementedException();
            }
            
        }

        public Boolean AreButtonsPressed(MouseButtons button1, 
                                         MouseButtons button2)
        {
            return false;
        }

        public Boolean AreButtonsPressed(MouseButtons button1, 
                                         MouseButtons button2, 
                                         MouseButtons button3)
        {
            return false;
        }

        public IPoint2D CursorPosition
        {
            get
            {
                Native.GetCursorPos(out var lpPoint);
                Point2D point2D = lpPoint;
                var offset = _offsetter.GetOffset(point2D);
                return offset;
            }
        }

        public Boolean IsCapsLockOn
        {
            get
            {
                var retVal = Native.GetKeyState((Int32) KeyboardButtons.CapsLock);
                return (retVal & 1) == 1;
            }
        }

        public Boolean IsMousePresent => true; //todo: better answer

        public Double MaximumFlingVelocity => 9000;

        public Double MinimumFlingVelocity => 90;

        public Boolean TryCaptureMouseInput(IVisualElement view)
        {
            Native.SetCapture(_windowHandle);
            
            return _inputHandler.TryCaptureMouseInput(view);
        }

        public Boolean TryReleaseMouseCapture(IVisualElement view)
        { 
            Native.ReleaseCapture();
            return _inputHandler.TryReleaseMouseCapture(view);
        }

        public IVisualElement? GetVisualWithMouseCapture()
        {
            return _inputHandler.GetVisualWithMouseCapture();
        }


        protected readonly IInputHandler _inputHandler;
        private readonly IntPtr _windowHandle;
        private readonly IPositionOffseter _offsetter;
    }
}