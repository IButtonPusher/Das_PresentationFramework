using System;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;

namespace Das.Views.Images.Svg;

/// <summary>
///     https://github.com/svg-net
/// </summary>
public sealed class SvgQuadraticCurveSegment : SvgPathSegment
{
   public SvgQuadraticCurveSegment(IPoint2F start,
                                   IPoint2F controlPoint,
                                   IPoint2F end)
      : base(start, end)
   {
      ControlPoint = controlPoint;
   }

   public IPoint2F ControlPoint { get; set; }

   private IPoint2F FirstControlPoint
   {
      get
      {
         var x1 = Start.X + (ControlPoint.X - Start.X) * 2 / 3;
         var y1 = Start.Y + (ControlPoint.Y - Start.Y) * 2 / 3;

         return new ValuePoint2F(x1, y1);
      }
   }

   private IPoint2F SecondControlPoint
   {
      get
      {
         var x2 = ControlPoint.X + (End.X - ControlPoint.X) / 3;
         var y2 = ControlPoint.Y + (End.Y - ControlPoint.Y) / 3;

         return new ValuePoint2F(x2, y2);
      }
   }

   public override void AddToPath(IGraphicsPath graphicsPath)
   {
      graphicsPath.AddBezier(Start, FirstControlPoint, SecondControlPoint, End);
   }

   public override String ToString()
   {
      return "Q" + ControlPoint.ToSvgString() + " " + End.ToSvgString();
   }
}