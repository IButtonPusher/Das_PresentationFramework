using System;
using System.Threading.Tasks;
using Das.Extensions;

namespace Das.Views.Core.Geometry
{
    public class Point2D : IPoint2D, 
                           IDeepCopyable<Point2D>
    {
        static Point2D()
        {
            Empty = new Point2D();
        }

        public Point2D()
        {
            IsOrigin = true;
        }

        public Point2D(Double x, 
                       Double y)
        {
            X = x;
            Y = y;
            IsOrigin = x.IsZero() && y.IsZero();
        }

        public Point2D DeepCopy()
        {
            return new Point2D(X, Y);
        }

        public Double X { get; }

        public Double Y { get; }

        public IPoint2D Offset(IPoint2D offset)
        {
            return new ValuePoint2D(X - offset.X, Y - offset.Y);
        }

        public IPoint2D Offset(Double x, 
                               Double y)
        {
            return new ValuePoint2D(X - x, Y - y);
        }

        public IPoint2D Offset(Double pct)
        {
            return GeometryHelper.Offset(this, pct);
        }


        public Boolean IsOrigin { get; }

        IPoint2D IDeepCopyable<IPoint2D>.DeepCopy()
        {
            return DeepCopy();
        }

        public static Point2D Empty { get; }

        public static IPoint2D operator +(Point2D p1, IPoint2D p2)
        {
            if (p2 == null)
                return p1.DeepCopy();

            if (p1 == null)
                return p2.DeepCopy();

            return new Point2D(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Size operator -(Point2D p1, Point2D p2)
        {
            if (p2 == null)
                return new Size(p1.X, p1.Y);

            if (p1 == null)
                return new Size(0 - p2.X, 0 - p2.Y);

            return new Size(p1.X - p2.X, p1.Y - p2.Y);
        }

        public override String ToString()
        {
            return X + ", " + Y;
        }
    }
}