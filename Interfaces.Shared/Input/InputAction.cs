using System;
using System.Threading.Tasks;

namespace Das.Views.Input
{
    [Flags]
    public enum InputAction
    {
        None,
        MouseOver = 1,
        LeftMouseButtonDown = 2,
        RightMouseButtonDown = 8,
        MouseButtonDown = LeftMouseButtonDown | RightMouseButtonDown,
        LeftMouseButtonUp = 16,
        RightMouseButtonUp = 32,
        LeftClick = 64,
        RightClick = 128,
        MiddleClick = 256,
        AnyMouseButton = LeftMouseButtonDown | RightMouseButtonDown | 
                         LeftMouseButtonUp  | RightMouseButtonUp | 
                         LeftClick | RightClick | MiddleClick,
        MouseDrag = 512,
        MouseWheel = 1024,
        Fling = 2056
    }
}