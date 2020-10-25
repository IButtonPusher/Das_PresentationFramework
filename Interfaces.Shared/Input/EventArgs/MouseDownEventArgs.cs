using System;
using Das.Views.Core.Geometry;

namespace Das.Views.Input
{
    public readonly struct MouseDownEventArgs : IMouseInputEventArgs<MouseDownEventArgs>
    {
        public MouseDownEventArgs(IPoint2D position, 
                              MouseButtons? button, 
                              IInputContext inputContext)
        {
            Position = position;
            Button = button;
            InputContext = inputContext;
        }

        public IPoint2D Position { get; }
        
        public readonly MouseButtons? Button;

        public MouseDownEventArgs Offset(IPoint2D position)
        {
            return new MouseDownEventArgs(Position.Offset(position), 
                Button, InputContext);
        }

        public InputAction Action => InputAction.MouseDown;

        public IInputContext InputContext { get; }
    }
}
