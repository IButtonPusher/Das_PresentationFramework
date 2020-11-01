using System;
using Das.Views.Core.Geometry;

namespace Das.Views.Input
{
    public readonly struct MouseUpEventArgs : IMouseButtonEventArgs<MouseUpEventArgs>
    {
        public MouseUpEventArgs(IPoint2D position, 
                              MouseButtons button, 
                              IInputContext inputContext)
        {
            Position = position;
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
        
        public MouseButtons Button { get; }

        public MouseUpEventArgs Offset(IPoint2D position)
        {
            return new MouseUpEventArgs(Position.Offset(position), 
                Button, InputContext);
        }

        public InputAction Action { get; }

        public IInputContext InputContext { get; }
    }
}
