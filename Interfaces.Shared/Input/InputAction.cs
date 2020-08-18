using System;

namespace Das.Views.Input
{
    [Flags]
    public enum InputAction
    {
        None,
        MouseHover = 1,
        MouseDown = 2,
        MouseUp = 4,
        LeftClick = 8
    }
}
