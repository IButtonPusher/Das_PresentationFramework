using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.Views.Extended.Core
{
    public class Triangle : IMultiLine
    {
        public Triangle(IPoint2D point2DA,
                        IPoint2D point2DB,
                        IPoint2D point2DC)
        {
            Point2DA = point2DA;
            Point2DB = point2DB;
            Point2DC = point2DC;
            PointArray = new[] {point2DA, point2DB, point2DC, point2DA};
        }

        public IEnumerator<IPoint2D> GetEnumerator()
        {
            yield return Point2DA;
            yield return Point2DB;
            yield return Point2DC;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IPoint2D[] PointArray { get; }

        public IPoint2D Point2DA { get; }

        public IPoint2D Point2DB { get; }

        public IPoint2D Point2DC { get; }

        public static implicit operator IPoint2D[](Triangle t)
        {
            return t.PointArray;
        }

        public override String ToString()
        {
            return $"{Point2DA}, {Point2DB}, {Point2DC}";
        }
    }
}