using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.Views.Input
{
    public readonly struct MouseClickEventArgs : IMouseInputEventArgs
    {
        public MouseClickEventArgs(IPoint2D position,
                                   MouseButtons button,
                                   Int32 clickCount,
                                   IInputContext inputContext)
        {
            Position = position;
            InputContext = inputContext;
            Button = button;
            ClickCount = clickCount;

            switch (button)
            {
                case MouseButtons.Left:
                    Action = InputAction.LeftClick;
                    break;

                case MouseButtons.Right:
                    Action = InputAction.RightClick;
                    break;

                case MouseButtons.Middle:
                    Action = InputAction.MiddleClick;
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        public readonly MouseButtons Button;
        public readonly Int32 ClickCount;

        public IPoint2D Position { get; }

        public InputAction Action { get; }

        public IInputContext InputContext { get; }
    }
}