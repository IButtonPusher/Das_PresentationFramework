using System;
using System.Threading.Tasks;

namespace Das.Views.Core.Geometry;

public interface IPoint2D : IDeepCopyable<IPoint2D>,
                            IEquatable<IPoint2D>
{
   Double X { get; }

   Double Y { get; }

   IPoint2D Offset(IPoint2D offset);

   IPoint2D Offset(Double x,
                   Double y);

   IPoint2D Offset(Double pct);

   Boolean IsOrigin { get; }
}