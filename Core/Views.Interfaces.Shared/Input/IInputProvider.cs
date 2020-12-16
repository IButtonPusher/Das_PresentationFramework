using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.Core.Input;

namespace Das.Views.Input
{
    public interface IInputProvider
    {
        IPoint2D CursorPosition { get; }

        Boolean IsCapsLockOn { get; }

        Boolean AreButtonsPressed(KeyboardButtons button1,
                                  KeyboardButtons button2);

        Boolean AreButtonsPressed(KeyboardButtons button1,
                                  KeyboardButtons button2,
                                  KeyboardButtons button3);

        Boolean AreButtonsPressed(MouseButtons button1,
                                  MouseButtons button2);

        Boolean AreButtonsPressed(MouseButtons button1,
                                  MouseButtons button2,
                                  MouseButtons button3);

        Boolean IsButtonPressed(KeyboardButtons keyboardButton);

        Boolean IsButtonPressed(MouseButtons mouseButton);
    }
}