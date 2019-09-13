using System.Collections;
using System.Collections.Generic;
using Das.Views.Core.Geometry;

namespace Das.Views.Extended.Core
{
    public class Triangle : IMultiLine
    {
        public Triangle(IPoint pointA, IPoint pointB, IPoint pointC)
        {
            PointA = pointA;
            PointB = pointB;
            PointC = pointC;
            _asArray = new[] { pointA, pointB, pointC };
        }

        public override string ToString() => $"{PointA}, {PointB}, {PointC}";

        public IPoint PointA { get; }
        public IPoint PointB { get; }
        public IPoint PointC { get; }

        private readonly IPoint[] _asArray;

        public IEnumerator<IPoint> GetEnumerator()
        {
            yield return PointA;
            yield return PointB;
            yield return PointC;
        }

        public static implicit operator IPoint[](Triangle t) => t._asArray;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IPoint[] PointArray => _asArray;
    }
}
