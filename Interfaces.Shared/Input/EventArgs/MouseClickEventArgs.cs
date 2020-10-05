using System;
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
        }

        public readonly MouseButtons Button;
        public readonly Int32 ClickCount;

        public IPoint2D Position { get; }
        public IInputContext InputContext { get; }
    }
}
