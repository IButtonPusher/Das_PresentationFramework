using System;
using Das.Views.Core.Geometry;

namespace Das.Views.Input
{
    public readonly struct MouseUpEventArgs : IMouseInputEventArgs
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

        public IInputContext InputContext { get; }
    }
}
