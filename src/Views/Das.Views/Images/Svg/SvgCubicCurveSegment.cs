using System;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;

namespace Das.Views.Images.Svg;

/// <summary>
///     https://github.com/svg-net
/// </summary>
public sealed class SvgCubicCurveSegment : SvgPathSegment
{
   public SvgCubicCurveSegment(IPoint2F start,
                               IPoint2F firstControlPoint,
                               IPoint2F secondControlPoint,
                               IPoint2F end)
      : base(start, end)
   {
      FirstControlPoint = firstControlPoint;
      SecondControlPoint = secondControlPoint;
   }

   public IPoint2F FirstControlPoint { get; set; }

   public IPoint2F SecondControlPoint { get; set; }

   public override void AddToPath(IGraphicsPath graphicsPath)
   {
      graphicsPath.AddBezier(Start, FirstControlPoint, SecondControlPoint, End);
   }

   public override String ToString()
   {
      return "C" + FirstControlPoint.ToSvgString() + " " + SecondControlPoint.ToSvgString() + " " +
             End.ToSvgString();
   }
}