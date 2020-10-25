using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.Views.Input
{
    public readonly struct MouseWheelEventArgs : IMouseInputEventArgs<MouseWheelEventArgs>
    {
        public MouseWheelEventArgs(IPoint2D position, 
                                   Int32 delta, 
                                   IInputContext inputContext)
        {
            Position = position;
            Delta = delta;
            InputContext = inputContext;
        }

        public IPoint2D Position { get;  }
        public readonly Int32 Delta;

        public MouseWheelEventArgs Offset(IPoint2D offset)
        {
            return new MouseWheelEventArgs(Position.Offset(offset), Delta, InputContext);
        }

        public InputAction Action => InputAction.MouseWheel;

        public IInputContext InputContext { get; }
    }
}