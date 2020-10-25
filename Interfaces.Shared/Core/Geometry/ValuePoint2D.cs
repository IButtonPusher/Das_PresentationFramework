using System;

namespace Das.Views.Core.Geometry
{
    public readonly struct ValuePoint2D : IPoint2D
    {
        public ValuePoint2D(Double x, Double y)
        {
            X =x;
            Y = y;
        }

        public Double X { get; }
        public Double Y { get; }

        public IPoint2D DeepCopy()
        {
            return new ValuePoint2D(X, Y);
        }

        public override String ToString()
        {
            return GetType().Name + " X: " + X + ", " + Y;
        }

        public IPoint2D Offset(IPoint2D offset)
        {
            return new ValuePoint2D(X - offset.X, Y - offset.Y);
        }

    }
}
