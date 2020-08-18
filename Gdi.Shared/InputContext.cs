using System;
using System.Windows.Forms;
using Das.Views.Core.Geometry;
using Windows.Shared;
using Windows.Shared.Input;
using Das.Views.Input;

namespace Das.Gdi
{
    public class InputContext : Win32InputContext, IMessageFilter
    {
        
        private readonly IPositionOffseter _offsetter;

        public InputContext(IPositionOffseter offsetter,
                            IInputHandler inputHandler)
        : base(offsetter, inputHandler)
        {
            _offsetter = offsetter;
            Application.AddMessageFilter(this);
        }

        //public Boolean AreButtonsPressed(MouseButtons button1,
        //    MouseButtons button2, MouseButtons button3) => false;

        //public IPoint CursorPosition
        //{
        //    get
        //    {
        //        Native.GetCursorPos(out var lpPoint);
        //        Point point = lpPoint;
        //        var offset = _offsetter.GetOffset(point);
        //        return offset;
        //    }
        //}

        //public Boolean IsButtonPressed(KeyboardButtons keyboardButton)
        //{
        //    switch (keyboardButton)
        //    {
        //        case KeyboardButtons.Control:
        //            return IsButtonPressed(KeyboardButtons.LControlKey) ||
        //                   IsButtonPressed(KeyboardButtons.RControlKey);
        //        case KeyboardButtons.Shift:
        //            return IsButtonPressed(KeyboardButtons.LShiftKey) ||
        //                   IsButtonPressed(KeyboardButtons.RShiftKey);
        //        default:
        //            var retVal = Native.GetKeyState((Int32) keyboardButton);
        //            return (retVal & 0x8000) == 0x8000;
        //    }
        //}

        //public Boolean AreButtonsPressed(KeyboardButtons button1, KeyboardButtons button2)
        //    => IsButtonPressed(button1) && IsButtonPressed(button2);

        //public Boolean AreButtonsPressed(KeyboardButtons button1, KeyboardButtons button2,
        //    KeyboardButtons button3) => AreButtonsPressed(
        //                                    button1, button2) && IsButtonPressed(button3);

        //public Boolean IsButtonPressed(MouseButtons mouseButton) => false;

        //public Boolean AreButtonsPressed(MouseButtons button1, MouseButtons button2) => false;

        //public Boolean IsCapsLockOn
        //{
        //    get
        //    {
        //        var retVal = Native.GetKeyState((Int32) KeyboardButtons.CapsLock);
        //        return (retVal & 1) == 1;
        //    }
        //}

      
        

        //protected abstract void OnMouseDown(MouseButtons button, IPoint position);

        //protected abstract void OnMouseUp(MouseButtons button, IPoint position);

        //protected abstract void OnKeyboardStateChanged();

        public Boolean PreFilterMessage(ref Message m)
        {
            ProcessMessage((MessageTypes)m.Msg);

            //switch ((MessageTypes)m.Msg)
            //{
            //    case MessageTypes.WM_LBUTTONDOWN:
            //        OnMouseDown(MouseButtons.Left, CursorPosition);
            //        break;
            //    case MessageTypes.WM_RBUTTONDOWN:
            //        OnMouseDown(MouseButtons.Right, CursorPosition);
            //        break;

            //    case MessageTypes.WM_LBUTTONUP:
            //        OnMouseUp(MouseButtons.Left, CursorPosition);
            //        break;
            //    case MessageTypes.WM_RBUTTONUP:
            //        OnMouseUp(MouseButtons.Right, CursorPosition);
            //        break;

            //    case MessageTypes.WM_KEYDOWN:
            //        OnKeyboardStateChanged();
            //        break;
            //}

            return false;
        }

        //public Boolean IsMousePresent => throw new NotImplementedException();
    }
}