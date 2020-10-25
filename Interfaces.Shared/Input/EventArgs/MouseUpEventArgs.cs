using System;
using Das.Views.Core.Geometry;

namespace Das.Views.Input
{
    public readonly struct MouseUpEventArgs : IMouseInputEventArgs<MouseUpEventArgs>
    {
        public MouseUpEventArgs(IPoint2D position, 
                              MouseButtons? button, 
                              IInputContext inputContext)
        {
            Position = position;
            Button = button;
            InputContext = inputContext;
        }

        public IPoint2D Position { get; }
        
        public readonly MouseButtons? Button;

        public MouseUpEventArgs Offset(IPoint2D position)
        {
            return new MouseUpEventArgs(Position.Offset(position), 
                Button, InputContext);
        }

        public InputAction Action => InputAction.MouseUp;

        public IInputContext InputContext { get; }
    }
}
