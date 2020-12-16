using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Das.Extensions;

namespace Das.Views.Core.Geometry
{
    public readonly struct ValuePoint2D : IPoint2D
    {
        [DebuggerStepThrough]
        [DebuggerHidden]
        public ValuePoint2D(Double x, 
                            Double y)
        {
            X = x;
            Y = y;
            IsOrigin = x.IsZero() && y.IsZero();
        }

        public static readonly ValuePoint2D Empty = new ValuePoint2D(0, 0);

        public Double X { get; }

        public Double Y { get; }

        public IPoint2D DeepCopy()
        {
            return new ValuePoint2D(X, Y);
        }

        public IPoint2D Offset(Double pct)
        {
            return GeometryHelper.Offset(this, pct);
        }

        public IPoint2D Offset(Double x, 
                               Double y)
        {
            return new ValuePoint2D(X - x, Y - y);
        }

        public static ValuePoint2D operator -(ValuePoint2D left,
            ValuePoint2D right)
        {
            return new ValuePoint2D(left.X - right.X,
                left.Y - right.Y);
        }

        public static ValuePoint2D operator /(ValuePoint2D left,
                                              Double right)
        {
            return new ValuePoint2D(left.X / right, left.Y  / right);
        }

        public override String ToString()
        {
            return GetType().Name + " X: " + X + ", " + Y;
        }

        public IPoint2D Offset(IPoint2D offset)
        {
            return new ValuePoint2D(X - offset.X, Y - offset.Y);
        }

        public Boolean IsOrigin { get; }
    }
}