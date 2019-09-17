using System;
using Das.Views.Core.Geometry;
using Das.Views.Core.Input;

namespace Das.Views.Input
{
    public interface IInputProvider
    {
        Boolean IsButtonPressed(KeyboardButtons keyboardButton);

        Boolean AreButtonsPressed(KeyboardButtons button1, KeyboardButtons button2);

        Boolean AreButtonsPressed(KeyboardButtons button1, KeyboardButtons button2,
            KeyboardButtons button3);

        Boolean IsButtonPressed(MouseButtons mouseButton);

        Boolean AreButtonsPressed(MouseButtons button1, MouseButtons button2);

        Boolean AreButtonsPressed(MouseButtons button1, MouseButtons button2,
            MouseButtons button3);

        IPoint CursorPosition { get; }

        Boolean IsCapsLockOn { get; }
    }
}