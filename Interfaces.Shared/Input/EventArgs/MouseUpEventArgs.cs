using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.Views.Input
{
    public readonly struct MouseUpEventArgs : IMouseButtonEventArgs<MouseUpEventArgs>
    {
        public MouseUpEventArgs(IPoint2D position,
                                IPoint2D? positionWentDown,
                                MouseButtons button,
                                IInputContext inputContext)
        {
            Position = position;
            PositionWentDown = positionWentDown;
            Button = button;
            InputContext = inputContext;

            switch (button)
            {
                case MouseButtons.Left:
                    Action = InputAction.LeftMouseButtonUp;
                    break;

                case MouseButtons.Right:
                    Action = InputAction.RightMouseButtonUp;
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        public IPoint2D Position { get; }

        public IPoint2D? PositionWentDown { get; }

        public MouseButtons Button { get; }

        public MouseUpEventArgs Offset(IPoint2D position)
        {
            return new MouseUpEventArgs(Position.Offset(position),
                PositionWentDown?.Offset(position),
                Button, InputContext);
        }

        public InputAction Action { get; }

        public IInputContext InputContext { get; }
    }
}