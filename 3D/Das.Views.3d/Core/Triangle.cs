using System;
using System.Collections;
using System.Collections.Generic;
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
            _asArray = new[] { point2DA, point2DB, point2DC, point2DA };
        }

        public override String ToString() => $"{Point2DA}, {Point2DB}, {Point2DC}";

        public IPoint2D Point2DA { get; }
        public IPoint2D Point2DB { get; }
        public IPoint2D Point2DC { get; }

        private readonly IPoint2D[] _asArray;

        public IEnumerator<IPoint2D> GetEnumerator()
        {
            yield return Point2DA;
            yield return Point2DB;
            yield return Point2DC;
        }

        public static implicit operator IPoint2D[](Triangle t) => t._asArray;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IPoint2D[] PointArray => _asArray;
    }
}
