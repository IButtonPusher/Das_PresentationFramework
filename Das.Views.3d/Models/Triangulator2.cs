using System;
using System.Collections.Generic;
using System.Diagnostics;
using Das.Views.Extended;

namespace Triangulation
{
    internal class ConnectionEdge
    {
        public readonly List<ConnectionEdge> OriginIncidentEdges;

        protected Boolean Equals(ConnectionEdge other)
        {
            return Next.Origin.Equals(other.Next.Origin) && Origin.Equals(other.Origin);
        }

        public override Boolean Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ConnectionEdge)obj);
        }

        public override Int32 GetHashCode()
        {
            unchecked
            {
                return ((Next.Origin != null ? Next.Origin.GetHashCode() : 0) * 397) ^ (Origin != null ? Origin.GetHashCode() : 0);
            }
        }

        internal Vector3 Origin { get; private set; }
        internal ConnectionEdge Prev;
        internal ConnectionEdge Next;
        internal Polygon Polygon { get; set; }

        public ConnectionEdge(Vector3 p0, 
                              Polygon parentPolygon,
                              List<ConnectionEdge> originIncidentEdges)
        {
            OriginIncidentEdges = originIncidentEdges;
            Origin = p0;
            Polygon = parentPolygon;
            AddIncidentEdge(this);
        }

        public override String ToString()
        {
            return "Origin: " + Origin + " Next: " + Next.Origin;
        }

        internal void AddIncidentEdge(ConnectionEdge next)
        {
            //var list = (List<ConnectionEdge>)Origin.DynamicProperties.GetValue(PropertyConstants.IncidentEdges);
            //list.Add(next);

            OriginIncidentEdges.Add(next);
        }
    }

    internal class Polygon
    {
        internal ConnectionEdge Start;
        internal Int32 PointCount = 0;

        internal IEnumerable<ConnectionEdge> GetPolygonCirculator()
        {
            if (Start == null) { yield break; }
            var h = Start;
            do
            {
                yield return h;
                h = h.Next;
            }
            while (h != Start);
        }

        internal void Remove(ConnectionEdge cur)
        {
            cur.Prev.Next = cur.Next;
            cur.Next.Prev = cur.Prev;
            //var incidentEdges = (List<ConnectionEdge>)cur.Origin.DynamicProperties.GetValue(PropertyConstants.IncidentEdges);
            var incidentEdges = cur.OriginIncidentEdges;

            Int32 index = incidentEdges.FindIndex(x => x.Equals(cur));
            Debug.Assert(index >= 0);
            incidentEdges.RemoveAt(index);
            if (incidentEdges.Count == 0)
                PointCount--;
            if (cur == Start)
                Start = cur.Prev;
        }

        public Boolean Contains(Vector3 vector2M, out Vector3 res)
        {
            foreach (var connectionEdge in GetPolygonCirculator())
            {
                if (connectionEdge.Origin.Equals(vector2M))
                {
                    res = connectionEdge.Origin;
                    return true;
                }
            }
            res = null!;
            return false;
        }
    }
}
