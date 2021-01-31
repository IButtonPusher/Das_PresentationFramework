using Das.Views.Extended;
using System;
using System.Collections.Generic;
// ReSharper disable All
#pragma warning disable 8601
#pragma warning disable 8602

namespace Triangulation
{
    public static class EarClipping
    {

        public static IEnumerable<Face> GetFaces(IList<Vector3> points, Int32[] vertexIndeces)
        {
            if (points.Count > 100000)
                yield break;

            //var res = new List<Vector3>();
            var current = new List<Vector3>();
            var mapping = new Dictionary<Vector3, Int32>();

            for (var c = 0; c < vertexIndeces.Length; c++)
            {
                if (vertexIndeces[c] >= 0)
                {
                    current.Add(points[vertexIndeces[c]]);
                    mapping.Add(points[vertexIndeces[c]], vertexIndeces[c]);
                }
                else
                {
                    current.Add(points[0 - vertexIndeces[c] - 1]);
                    mapping.Add(points[0 - vertexIndeces[c] - 1], 
                        0 - vertexIndeces[c] - 1);

                    var Normal = CalcNormal(current);

                    if (!Normal.Equals(Vector3.Zero))
                    {
                        var bob = Triangulate(current, Normal);

                        //Debug.WriteLine("Triangulating " + current.Count + " = " + bob.Count);

                        for (var i = 0; i + 3 <= bob.Count;)
                        {
                            yield return new Face(mapping[bob[i++]],
                                mapping[bob[i++]],
                                mapping[bob[i++]]);
                        }
                    }

                    current.Clear();
                    mapping.Clear();
                }
            }

            yield break;

        }


        public static Int32 GetOrientation(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 normal)
        {
           
            var res = (v0 - v1).Cross(v2 - v1);
            if (res.LengthSquared() == 0)
                return 0;
           
            if (Math.Sign(res.X) != Math.Sign(normal.X) || Math.Sign(res.Y) != Math.Sign(normal.Y) || Math.Sign(res.Z) != Math.Sign(normal.Z))
                return 1;
            return -1;
        }

        //public void SetPoints(List<Vector3> points, List<List<Vector3>> holes = null, Vector3 normal = null)
        //{
        //    if (points == null || points.Count < 3)
        //    {
        //        throw new ArgumentException("No list or an empty list passed");
        //    }
        //    if (normal == null)
        //        CalcNormal(points);
        //    else
        //    {
        //        Normal = normal;
        //    }
        //    _mainPointList = new Polygon();
        //    LinkAndAddToList(_mainPointList, points);

        //    if (holes != null)
        //    {
        //        _holes = new List<Polygon>();
        //        for (int i = 0; i < holes.Count; i++)
        //        {
        //            Polygon p = new Polygon();
        //            LinkAndAddToList(p, holes[i]);
        //            _holes.Add(p);
        //        }
        //    }
        //}

        // calculating normal using Newell's method
        private static Vector3 CalcNormal(List<Vector3> points)
        {
            Vector3 normal = Vector3.Zero;
            for (var i = 0; i < points.Count; i++)
            {
                var j = (i + 1) % (points.Count);
                normal.X += (points[i].Y - points[j].Y) * (points[i].Z + points[j].Z);
                normal.Y += (points[i].Z - points[j].Z) * (points[i].X + points[j].X);
                normal.Z += (points[i].X - points[j].X) * (points[i].Y + points[j].Y);
            }

            return normal;
        }

        private static void LinkAndAddToList(Polygon polygon, 
                                             List<Vector3> points,
                                             Dictionary<Vector3, List<ConnectionEdge>> incidentEdges)
        {
            ConnectionEdge? prev = null, first = null;
            Dictionary<Vector3, Vector3> pointsHashSet = new Dictionary<Vector3, Vector3>();
            Int32 pointCount = 0;
            for (Int32 i = 0; i < points.Count; i++)
            {
                // we don't wanna have duplicates
                Vector3 p0;
                if (pointsHashSet.ContainsKey(points[i]))
                {
                    p0 = pointsHashSet[points[i]];
                }
                else
                {
                    p0 = points[i];
                    pointsHashSet.Add(p0, p0);
                    List<ConnectionEdge> list = new List<ConnectionEdge>();
                    incidentEdges[p0] = list;
                    //p0.DynamicProperties.AddProperty(PropertyConstants.IncidentEdges, list);
                    pointCount++;
                }
                ConnectionEdge current = new ConnectionEdge(p0, polygon, incidentEdges[p0]);
                first = (i == 0) ? current : first; // remember first
                if (prev != null)
                {
                    prev.Next = current;
                }
                current.Prev = prev;
                prev = current;
            }
            first.Prev = prev;
            prev.Next = first;
            polygon.Start = first;
            polygon.PointCount = pointCount;
        }

        

        public static List<Vector3> Triangulate(List<Vector3> points, Vector3 Normal)
        {
            if (points == null || points.Count < 3)
            {
                throw new ArgumentException("No list or an empty list passed");
            }

            var incidentEdges = new Dictionary<Vector3, List<ConnectionEdge>>();

            var _mainPointList = new Polygon();
            LinkAndAddToList(_mainPointList, points,incidentEdges);

            //////////////////////////////////////////////


            var Result = new List<Vector3>();

            if (Normal.Equals(Vector3.Zero))
                throw new Exception("The input is not a valid polygon");
            //if (_holes != null && _holes.Count > 0)
            //{
            //    ProcessHoles();
            //}

            List<ConnectionEdge> nonConvexPoints = FindNonConvexPoints(_mainPointList, Normal);

            if (nonConvexPoints.Count == _mainPointList.PointCount)
                throw new ArgumentException("The triangle input is not valid");

            while (_mainPointList.PointCount > 2)
            {
                Boolean guard = false;
                foreach (var cur in _mainPointList.GetPolygonCirculator())
                {
                    if (!IsConvex(cur, Normal))
                        continue;

                    if (!IsPointInTriangle(cur.Prev.Origin, cur.Origin, cur.Next.Origin, nonConvexPoints,
                        Normal))
                    {
                        // cut off ear
                        guard = true;
                        Result.Add(cur.Prev.Origin);
                        Result.Add(cur.Origin);
                        Result.Add(cur.Next.Origin);

                        // Check if prev and next are still nonconvex. If not, then remove from non convex list
                        if (IsConvex(cur.Prev, Normal))
                        {
                            Int32 index = nonConvexPoints.FindIndex(x => x == cur.Prev);
                            if (index >= 0)
                                nonConvexPoints.RemoveAt(index);
                        }
                        if (IsConvex(cur.Next, Normal))
                        {
                            Int32 index = nonConvexPoints.FindIndex(x => x == cur.Next);
                            if (index >= 0)
                                nonConvexPoints.RemoveAt(index);
                        }
                        _mainPointList.Remove(cur);
                        break;
                    }
                }

                if (PointsOnLine(_mainPointList, Normal))
                    break;
                if (!guard)
                {
                    return new List<Vector3>();
                }
            }

            return Result;
        }

        private static Boolean PointsOnLine(Polygon pointList, Vector3 Normal)
        {
            foreach (var connectionEdge in pointList.GetPolygonCirculator())
            {
                if (GetOrientation(connectionEdge.Prev.Origin, connectionEdge.Origin, 
                    connectionEdge.Next.Origin, Normal) != 0)
                    return false;
            }
            return true;
        }

        private static Boolean IsConvex(ConnectionEdge curPoint, Vector3 Normal)
        {
            Int32 orientation = GetOrientation(curPoint.Prev.Origin, curPoint.Origin, curPoint.Next.Origin,
                Normal);
            return orientation == 1;
        }

        //private void ProcessHoles()
        //{
        //    for (int h = 0; h < _holes.Count; h++)
        //    {
        //        List<Polygon> polygons = new List<Polygon>();
        //        polygons.Add(_mainPointList);
        //        polygons.AddRange(_holes);
        //        ConnectionEdge M, P;
        //        GetVisiblePoints(h + 1, polygons, out M, out P);
        //        if (M.Origin.Equals(P.Origin))
        //            throw new Exception();

        //        var insertionEdge = P;
        //        InsertNewEdges(insertionEdge, M);
        //        _holes.RemoveAt(h);
        //        h--;
        //    }
        //}

        //private void InsertNewEdges(ConnectionEdge insertionEdge, ConnectionEdge m)
        //{
        //    insertionEdge.Polygon.PointCount += m.Polygon.PointCount;
        //    var cur = m;
        //    var forwardEdge = new ConnectionEdge(insertionEdge.Origin, insertionEdge.Polygon);
        //    forwardEdge.Prev = insertionEdge.Prev;
        //    forwardEdge.Prev.Next = forwardEdge;
        //    forwardEdge.Next = m;
        //    forwardEdge.Next.Prev = forwardEdge;
        //    var end = insertionEdge;
        //    ConnectionEdge prev = null;
        //    do
        //    {
        //        cur.Polygon = insertionEdge.Polygon;
        //        prev = cur;
        //        cur = cur.Next;
        //    } while (m != cur);
        //    var backEdge = new ConnectionEdge(cur.Origin, insertionEdge.Polygon);
        //    cur = prev;
        //    cur.Next = backEdge;
        //    backEdge.Prev = cur;
        //    backEdge.Next = end;
        //    end.Prev = backEdge;
        //}

        //private void GetVisiblePoints(int holeIndex, List<Polygon> polygons, 
        //                              out ConnectionEdge M, 
        //                              out ConnectionEdge P,
        //                              Vector3 Normal)
        //{
        //    M = FindLargest(polygons[holeIndex], Normal);

        //    var direction = (polygons[holeIndex].Start.Next.Origin - polygons[holeIndex].Start.Origin).Cross(Normal);
        //    var I = FindPointI(M, polygons, holeIndex, direction,Normal);

        //    Vector3 res;
        //    if (polygons[I.PolyIndex].Contains(I.I, out res))
        //    {
        //        var incidentEdges =
        //            (List<ConnectionEdge>)res.DynamicProperties.GetValue(PropertyConstants.IncidentEdges);
        //        foreach (var connectionEdge in incidentEdges)
        //        {
        //            if (IsBetween(connectionEdge.Origin, connectionEdge.Next.Origin, connectionEdge.Prev.Origin, M.Origin, Normal) == 1)
        //            {
        //                P = connectionEdge;
        //                return;
        //            }
        //        }
        //        throw new Exception();
        //    }
        //    else
        //    {
        //        P = FindVisiblePoint(I, polygons, M, direction,Normal);
        //    }
        //}

        //private ConnectionEdge FindVisiblePoint(Candidate I, 
        //                                        List<Polygon> polygons, 
        //                                        ConnectionEdge M, 
        //                                        Vector3 direction,
        //                                        Vector3 Normal)
        //{
        //    ConnectionEdge P = null;

        //    if (I.Origin.Origin.X > I.Origin.Next.Origin.X)
        //    {
        //        P = I.Origin;
        //    }
        //    else
        //    {
        //        P = I.Origin.Next;
        //    }

        //    List<ConnectionEdge> nonConvexPoints = FindNonConvexPoints(polygons[I.PolyIndex],Normal);


        //    nonConvexPoints.Remove(P);

        //    var m = M.Origin;
        //    var i = I.I;
        //    var p = P.Origin;
        //    List<ConnectionEdge> candidates = new List<ConnectionEdge>();

        //    // invert i and p if triangle is oriented CW
        //    if (GetOrientation(m, i, p, Normal) == -1)
        //    {
        //        var tmp = i;
        //        i = p;
        //        p = tmp;
        //    }

        //    foreach (var nonConvexPoint in nonConvexPoints)
        //    {
        //        if (PointInOrOnTriangle(m, i, p, nonConvexPoint.Origin, Normal))
        //        {
        //            candidates.Add(nonConvexPoint);
        //        }
        //    }
        //    if (candidates.Count == 0)
        //        return P;
        //    return FindMinimumAngle(candidates, m, direction);
        //}

        //private ConnectionEdge FindMinimumAngle(List<ConnectionEdge> candidates, Vector3 M, Vector3 direction)
        //{
        //    double angle = -double.MaxValue;
        //    ConnectionEdge result = null;
        //    foreach (var R in candidates)
        //    {
        //        var a = direction;
        //        var b = R.Origin - M;
        //        var num = a.Dot(b) * a.Dot(b);
        //        var denom = b.Dot(b);
        //        var res = num / denom;
        //        if (res > angle)
        //        {
        //            result = R;
        //            angle = res;
        //        }
        //    }
        //    return result;
        //}

        //// Is testPoint between a and b in ccw order?
        //// > 0 if strictly yes
        //// < 0 if strictly no
        //// = 0 if testPoint lies either on a or on b
        //public static int IsBetween(Vector3 Origin, Vector3 a, Vector3 b, Vector3 testPoint, Vector3 normal)
        //{
        //    var psca = GetOrientation(Origin, a, testPoint, normal);
        //    var pscb = GetOrientation(Origin, b, testPoint, normal);

        //    // where does b in relation to a lie? Left, right or collinear?
        //    var psb = GetOrientation(Origin, a, b, normal);
        //    if (psb > 0) // left
        //    {
        //        // if left then testPoint lies between a and b iff testPoint left of a AND testPoint right of b
        //        if (psca > 0 && pscb < 0)
        //            return 1;
        //        if (psca == 0)
        //        {
        //            var t = a - Origin;
        //            var t2 = testPoint - Origin;
        //            if (Math.Sign(t.X) != Math.Sign(t2.X) || Math.Sign(t.Y) != Math.Sign(t2.Y))
        //                return -1;
        //            return 0;
        //        }
        //        else if (pscb == 0)
        //        {
        //            var t = b - Origin;
        //            var t2 = testPoint - Origin;
        //            if (Math.Sign(t.X) != Math.Sign(t2.X) || Math.Sign(t.Y) != Math.Sign(t2.Y))
        //                return -1;
        //            return 0;
        //        }
        //    }
        //    else if (psb < 0) // right
        //    {
        //        // if right then testPoint lies between a and b iff testPoint left of a OR testPoint right of b
        //        if (psca > 0 || pscb < 0)
        //            return 1;
        //        if (psca == 0)
        //        {
        //            var t = a - Origin;
        //            var t2 = testPoint - Origin;
        //            if (Math.Sign(t.X) != Math.Sign(t2.X) || Math.Sign(t.Y) != Math.Sign(t2.Y))
        //                return 1;
        //            return 0;
        //        }
        //        else if (pscb == 0)
        //        {
        //            var t = b - Origin;
        //            var t2 = testPoint - Origin;
        //            if (Math.Sign(t.X) != Math.Sign(t2.X) || Math.Sign(t.Y) != Math.Sign(t2.Y))
        //                return 1;
        //            return 0;
        //        }
        //    }
        //    else if (psb == 0)
        //    {
        //        if (psca > 0)
        //            return 1;
        //        else if (psca < 0)
        //            return -1;
        //        else
        //            return 0;
        //    }
        //    return -1;
        //}

        //private Candidate FindPointI(ConnectionEdge M, List<Polygon> polygons, int holeIndex, Vector3 direction,
        //                             Vector3 Normal)
        //{
        //    Candidate candidate = new Candidate();
        //    for (int i = 0; i < polygons.Count; i++)
        //    {
        //        if (i == holeIndex) // Don't test the hole with itself
        //            continue;
        //        foreach (var connectionEdge in polygons[i].GetPolygonCirculator())
        //        {
        //            double rayDistanceSquared;
        //            Vector3 intersectionPoint;

        //            if (RaySegmentIntersection(out intersectionPoint, out rayDistanceSquared, M.Origin, direction, connectionEdge.Origin,
        //                connectionEdge.Next.Origin, direction))
        //            {
        //                if (rayDistanceSquared == candidate.currentDistance)  // if this is an M/I edge, then both edge and his twin have the same distance; we take the edge where the point is on the left side
        //                {
        //                    if (GetOrientation(connectionEdge.Origin, connectionEdge.Next.Origin, M.Origin, 
        //                        Normal) == 1)
        //                    {
        //                        candidate.currentDistance = rayDistanceSquared;
        //                        candidate.Origin = connectionEdge;
        //                        candidate.PolyIndex = i;
        //                        candidate.I = intersectionPoint;
        //                    }
        //                }
        //                else if (rayDistanceSquared < candidate.currentDistance)
        //                {
        //                    candidate.currentDistance = rayDistanceSquared;
        //                    candidate.Origin = connectionEdge;
        //                    candidate.PolyIndex = i;
        //                    candidate.I = intersectionPoint;
        //                }
        //            }
        //        }

        //    }
        //    return candidate;
        //}

        //private ConnectionEdge FindLargest(Polygon testHole, Vector3 Normal)
        //{
        //    double maximum = 0;
        //    ConnectionEdge maxEdge = null;
        //    Vector3 v0 = testHole.Start.Origin;
        //    Vector3 v1 = testHole.Start.Next.Origin;
        //    foreach (var connectionEdge in testHole.GetPolygonCirculator())
        //    {
        //        // we take the first two points as a reference line

        //        if (GetOrientation(v0, v1, connectionEdge.Origin, Normal) < 0)
        //        {
        //            var r = PointLineDistance(v0, v1, connectionEdge.Origin);
        //            if (r > maximum)
        //            {
        //                maximum = r;
        //                maxEdge = connectionEdge;
        //            }
        //        }
        //    }
        //    if (maxEdge == null)
        //        return testHole.Start;
        //    return maxEdge;
        //}

        private static Boolean IsPointInTriangle(Vector3 prevPoint, Vector3 curPoint, Vector3 nextPoint, 
                                                 List<ConnectionEdge> nonConvexPoints,
                                                 Vector3 Normal)
        {
            foreach (var nonConvexPoint in nonConvexPoints)
            {
                if (nonConvexPoint.Origin == prevPoint || nonConvexPoint.Origin == curPoint || nonConvexPoint.Origin == nextPoint)
                    continue;
                if (PointInOrOnTriangle(prevPoint, curPoint, nextPoint, nonConvexPoint.Origin, Normal))
                    return true;
            }
            return false;
        }

        public static Boolean PointInOrOnTriangle(Vector3 prevPoint, Vector3 curPoint, Vector3 nextPoint, Vector3 nonConvexPoint, Vector3 normal)
        {
            var res0 = GetOrientation(prevPoint, nonConvexPoint, curPoint, normal);
            var res1 = GetOrientation(curPoint, nonConvexPoint, nextPoint, normal);
            var res2 = GetOrientation(nextPoint, nonConvexPoint, prevPoint, normal);
            return res0 != 1 && res1 != 1 && res2 != 1;
        }


        private static List<ConnectionEdge> FindNonConvexPoints(Polygon p,Vector3 Normal)
        {
            List<ConnectionEdge> resultList = new List<ConnectionEdge>();
            foreach (var connectionEdge in p.GetPolygonCirculator())
            {
                if (GetOrientation(connectionEdge.Prev.Origin, connectionEdge.Origin, 
                    connectionEdge.Next.Origin, Normal) != 1)
                    resultList.Add(connectionEdge);
            }
            return resultList;
        }

        //public static bool RaySegmentIntersection(out Vector3 intersection, out double distanceSquared, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint3, Vector3 linePoint4, Vector3 direction)
        //{
        //    var lineVec2 = linePoint4 - linePoint3;
        //    Vector3 lineVec3 = linePoint3 - linePoint1;
        //    Vector3 crossVec1and2 = lineVec1.Cross(lineVec2);
        //    Vector3 crossVec3and2 = lineVec3.Cross(lineVec2);

        //    var res = PointLineDistance(linePoint3, linePoint4, linePoint1);
        //    if (res == 0) // line and ray are collinear
        //    {
        //        var p = linePoint1 + lineVec1;
        //        var res2 = PointLineDistance(linePoint3, linePoint4, p);
        //        if (res2 == 0)
        //        {
        //            var s = linePoint3 - linePoint1;
        //            if (s.X == direction.X && s.Y == direction.Y && s.Z == direction.Z)
        //            {
        //                intersection = linePoint3;
        //                distanceSquared = s.LengthSquared();
        //                return true;
        //            }
        //        }
        //    }
        //    //is coplanar, and not parallel
        //    if (/*planarFactor == 0.0f && */crossVec1and2.LengthSquared() > 0)
        //    {
        //        var s = crossVec3and2.Dot(crossVec1and2) / crossVec1and2.LengthSquared();
        //        if (s >= 0)
        //        {
        //            intersection = linePoint1 + (lineVec1 * s);
        //            distanceSquared = (lineVec1 * s).LengthSquared();
        //            if ((intersection - linePoint3).LengthSquared() + (intersection - linePoint4).LengthSquared() <=
        //                lineVec2.LengthSquared())
        //                return true;
        //        }
        //    }
        //    intersection = Vector3.Zero;
        //    distanceSquared = 0;
        //    return false;
        //}

        //public static double PointLineDistance(Vector3 p1, Vector3 p2, Vector3 p3)
        //{
        //    return (p2 - p1).Cross(p3 - p1).LengthSquared();
        //}
    }

   
}