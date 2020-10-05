using System;
using System.Threading.Tasks;

namespace Das.Views.Input
{
    [Flags]
    public enum InputAction
    {
        None,
        MouseOver = 1,
        MouseDown = 2,
        MouseUp = 4,
        LeftClick = 8,
        MouseDrag = 16,
        MouseWheel = 32
    }
}