using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.Views.Input
{
    public readonly struct FlingEventArgs : IMouseInputEventArgs<FlingEventArgs>
    {
        public FlingEventArgs(Double velocityX,
                              Double velocityY,
                              IPoint2D position,
                              IInputContext inputContext,
                              Action? onFlingComplete)
        {
            VelocityX = velocityX;
            VelocityY = velocityY;
            _onFlingComplete = onFlingComplete;
            Position = position;
            InputContext = inputContext;
        }

        public readonly Double VelocityX;

        public readonly Double VelocityY;
        private readonly Action? _onFlingComplete;

        public IPoint2D Position { get; }

        public FlingEventArgs Offset(IPoint2D offset)
        {
            return new(VelocityX, VelocityY, Position.Offset(offset), InputContext, _onFlingComplete);
        }


        public void SetHandled(Boolean value)
        {
            if (_onFlingComplete is { } something)
                something();
        }

        public override String ToString()
        {
            return GetType().Name + " vX: " + VelocityX + " vY: " + VelocityY;
        }

        public InputAction Action => InputAction.Fling;

        public IInputContext InputContext { get; }
    }
}
