using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Das.Gdi.Core;
using Das.Views.Core.Geometry;
using Das.Views.Core.Input;
using Das.Views.Input;
using MouseButtons = Das.Views.Core.Input.MouseButtons;

namespace Das.Gdi
{
    public abstract class InputContext : IInputContext, IMessageFilter
    {
        //private readonly IInputHandler _inputHandler;
        private readonly IPositionOffseter _offsetter;

        public InputContext(IPositionOffseter offsetter)
        {
            // _inputHandler = inputHandler;
            _offsetter = offsetter;
            Application.AddMessageFilter(this);
        }

        public bool AreButtonsPressed(MouseButtons button1,
            MouseButtons button2, MouseButtons button3) => false;

        public IPoint CursorPosition
        {
            get
            {
                GetCursorPos(out var lpPoint);
                Point point = lpPoint;
                var offset = _offsetter.GetOffset(point);
                return offset;
            }
        }

        public bool IsButtonPressed(KeyboardButtons keyboardButton)
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
                    var retVal = GetKeyState((Int32) keyboardButton);
                    return (retVal & 0x8000) == 0x8000;
            }
        }

        public bool AreButtonsPressed(KeyboardButtons button1, KeyboardButtons button2)
            => IsButtonPressed(button1) && IsButtonPressed(button2);

        public bool AreButtonsPressed(KeyboardButtons button1, KeyboardButtons button2,
            KeyboardButtons button3) => AreButtonsPressed(
                                            button1, button2) && IsButtonPressed(button3);

        public bool IsButtonPressed(MouseButtons mouseButton) => false;

        public bool AreButtonsPressed(MouseButtons button1, MouseButtons button2) => false;

        public bool IsCapsLockOn
        {
            get
            {
                var retVal = GetKeyState((Int32) KeyboardButtons.CapsLock);
                return (retVal & 1) == 1;
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern short GetKeyState(int keyCode);


        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);


        protected abstract void OnMouseHovering(IPoint position);

        protected abstract void OnMouseDown(MouseButtons button, IPoint position);

        protected abstract void OnMouseUp(MouseButtons button, IPoint position);

        protected abstract void OnKeyboardStateChanged();

        public bool PreFilterMessage(ref Message m)
        {
            switch (m.Msg)
            {
                case Win32.WM_LBUTTONDOWN:
                    OnMouseDown(MouseButtons.Left, CursorPosition);
                    break;
                case Win32.WM_RBUTTONDOWN:
                    OnMouseDown(MouseButtons.Right, CursorPosition);
                    break;

                case Win32.WM_LBUTTONUP:
                    OnMouseUp(MouseButtons.Left, CursorPosition);
                    break;
                case Win32.WM_RBUTTONUP:
                    OnMouseUp(MouseButtons.Right, CursorPosition);
                    break;

                case Win32.WM_KEYDOWN:
                    OnKeyboardStateChanged();
                    break;
            }

            return false;
        }

        public bool IsMousePresent => throw new NotImplementedException();
    }
}