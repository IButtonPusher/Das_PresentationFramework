using System;

namespace Das.Views.Core.Geometry
{
    public class Point : IPoint, IDeepCopyable<Point>
    {
        public Double X { get; }
        public Double Y { get; }

        public Point()
        {
        }

        public Point(Double x, Double y)
        {
            X = x;
            Y = y;
        }

        static Point()
        {
            _empty = new Point();
        }

        private static readonly Point _empty;
        public static Point Empty => _empty;

        public Point DeepCopy() => new Point(X, Y);

        IPoint IDeepCopyable<IPoint>.DeepCopy() => DeepCopy();

        public override string ToString() => X + ", " + Y;

        public static IPoint operator +(Point p1, IPoint p2)
        {
            if (p2 == null)
                return p1.DeepCopy();

            if (p1 == null)
                return p2.DeepCopy();

            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Size operator -(Point p1, Point p2)
        {
            if (p2 == null)
                return new Size(p1.X, p1.Y);

            if (p1 == null)
                return new Size(0 - p2.X, 0 - p2.Y);

            return new Size(p1.X - p2.X, p1.Y - p2.Y);
        }
    }
}