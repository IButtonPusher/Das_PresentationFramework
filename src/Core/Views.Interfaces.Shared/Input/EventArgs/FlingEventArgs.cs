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
                              Action? onFlingComplete,
                              Single effectiveFriction,
                              Single physicalCoefficient)
        {
            VelocityX = velocityX;
            VelocityY = velocityY;
            _onFlingComplete = onFlingComplete;
            EffectiveFriction = effectiveFriction;
            Position = position;
            InputContext = inputContext;
            PhysicalCoefficient = physicalCoefficient;
        }

        public readonly Double VelocityX;
        public readonly Double VelocityY;

        public readonly Single EffectiveFriction;

        public readonly Single PhysicalCoefficient;

        private readonly Action? _onFlingComplete;

        public IPoint2D Position { get; }

        public FlingEventArgs Offset(IPoint2D offset)
        {
            return new(VelocityX, VelocityY, Position.Offset(offset), 
                InputContext, _onFlingComplete, EffectiveFriction, PhysicalCoefficient);
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
