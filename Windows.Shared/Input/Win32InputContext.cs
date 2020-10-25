using System;
using System.Threading.Tasks;
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
                                 IInputHandler inputHandler)
        {
            _offsetter = offsetter;
            _inputHandler = inputHandler;
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

        public Boolean AreButtonsPressed(KeyboardButtons button1, KeyboardButtons button2)
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
            return false;
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

        public Double MaximumFlingVelocity => 0;

        public Double MinimumFlingVelocity => 0;

        public Boolean TryCaptureMouseInput(IVisualElement view)
        {
            return _inputHandler.TryCaptureMouseInput(view);
        }

        public Boolean TryReleaseMouseCapture(IVisualElement view)
        {
            return _inputHandler.TryReleaseMouseCapture(view);
        }


        //internal static IntPtr GetWindowLongPtr(HandleRef hWnd, int nIndex)
        //{
        //    IntPtr num = IntPtr.Zero;
        //    int lastWin32Error;

        //    switch (IntPtr.Size)
        //    {
        //        case 4:
        //            var window32 = Native.GetWindowLongPtr32(hWnd.Handle, nIndex);
        //            lastWin32Error = Marshal.GetLastWin32Error();
        //            return window32;
        //            //num = new IntPtr(windowLong);

        //        case 8:
        //            var window64 = Native.GetWindowLongPtr64(hWnd.Handle, nIndex);
        //            lastWin32Error = Marshal.GetLastWin32Error();
        //            return window64;

        //        default:
        //            throw new Exception("Unsupported pointer size: " + IntPtr.Size);
        //    }
        //}

        //private IntPtr _hwndAttached;
        //private HandleRef _hwndHandleRef;
        //private Bond _bond;
        //private WndProc _attachedWndProc;
        //private IntPtr _oldWndProc;

        //private enum Bond
        //{
        //    Unattached,
        //    Attached,
        //    Detached,
        //    Orphaned,
        //}

        //private void HookWindowProc(IntPtr hwnd, WndProc newWndProc, IntPtr oldWndProc)
        //{
        //    this._hwndAttached = hwnd;
        //    this._hwndHandleRef = new HandleRef((object) null, this._hwndAttached);
        //    this._bond = Bond.Attached;
        //    this._attachedWndProc = newWndProc;
        //    this._oldWndProc = oldWndProc;
        //    //Native.SetWindowLongPtr(this._hwndHandleRef, -4, this._attachedWndProc);
        //    //ManagedWndProcTracker.TrackHwndSubclass(this, this._hwndAttached);
        //}

        //protected abstract void OnMouseDown(MouseButtons button, IPoint2D position);

        //protected abstract void OnMouseUp(MouseButtons button, IPoint2D position);

        //protected abstract void OnKeyboardStateChanged();

        //protected void ProcessMessage(MessageTypes messageType)
        //{
        //    switch (messageType)
        //    {
        //        case MessageTypes.WM_LBUTTONDOWN:
        //            _inputHandler.OnMouseDown(MouseButtons.Left, CursorPosition);
        //            break;
        //        case MessageTypes.WM_RBUTTONDOWN:
        //            _inputHandler.OnMouseDown(MouseButtons.Right, CursorPosition);
        //            break;

        //        case MessageTypes.WM_LBUTTONUP:
        //            _inputHandler.OnMouseUp(MouseButtons.Left, CursorPosition);
        //            break;
        //        case MessageTypes.WM_RBUTTONUP:
        //            _inputHandler.OnMouseUp(MouseButtons.Right, CursorPosition);
        //            break;

        //        case MessageTypes.WM_KEYDOWN:
        //            _inputHandler.OnKeyboardStateChanged();
        //            break;
        //    }
        //}

        protected readonly IInputHandler _inputHandler;
        private readonly IPositionOffseter _offsetter;
    }
}