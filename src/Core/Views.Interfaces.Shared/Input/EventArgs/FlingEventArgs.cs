using System;
using System.Threading;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.Views.Input
{
    public readonly struct FlingEventArgs : IMouseInputEventArgs<FlingEventArgs>,
                                            IEquatable<FlingEventArgs>
    {
        public FlingEventArgs(Double velocityX,
                              Double velocityY,
                              IPoint2D position,
                              IInputContext inputContext,
                              //Action? onFlingComplete,
                              //Single effectiveFriction,
                              //Single physicalCoefficient,
                              Double distanceFlungX,
                              Double distanceFlungY,
                              TimeSpan flingXDuration,
                              TimeSpan flingYDuration)
        {
            VelocityX = velocityX;
            VelocityY = velocityY;
            //_onFlingComplete = onFlingComplete;
            DistanceFlungX = distanceFlungX;
            DistanceFlungY = distanceFlungY;
            FlingXDuration = flingXDuration;
            FlingYDuration = flingYDuration;
            //EffectiveFriction = effectiveFriction;
            Position = position;
            InputContext = inputContext;
            //PhysicalCoefficient = physicalCoefficient;

            _id = Interlocked.Increment(ref _counter);
        }

        public readonly Double VelocityX;
        
        public readonly Double VelocityY;

        public readonly Double DistanceFlungY;
        public readonly TimeSpan FlingXDuration;
        public readonly TimeSpan FlingYDuration;

        public readonly Double DistanceFlungX;

        private readonly Int32 _id;

        public override Int32 GetHashCode()
        {
            return _id;
        }

        public Boolean Equals(FlingEventArgs fargs)
        {
            return fargs._id == _id;
        }

        public override Boolean Equals(Object obj)
        {
            return obj is FlingEventArgs fargs && fargs._id == _id;
        }

        //public readonly Single EffectiveFriction;

        //public readonly Single PhysicalCoefficient;

        //private readonly Action? _onFlingComplete;

        private static Int32 _counter;

        public IPoint2D Position { get; }

        public FlingEventArgs Offset(IPoint2D offset)
        {
            return new(VelocityX, VelocityY, Position.Offset(offset), 
                InputContext, //_onFlingComplete, 
                DistanceFlungX, DistanceFlungY,
                FlingXDuration, FlingYDuration);
        }


        //public void SetHandled(Boolean value)
        //{
        //    if (_onFlingComplete is { } something)
        //        something();
        //}

        public override String ToString()
        {
            return GetType().Name + " vX: " + VelocityX.ToString("0.00") + 
                   " vY: " + VelocityY.ToString("0.00") + 
                   " sumX: " + DistanceFlungX.ToString("0.00") + 
                   " sumY: " + DistanceFlungY.ToString("0.00") + 
                   " tX: " + FlingXDuration + " tY: " + FlingYDuration;
        }

        public InputAction Action => InputAction.Fling;

        public IInputContext InputContext { get; }
    }
}
