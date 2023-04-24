using System;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;

namespace Das.Views.Images.Svg;

/// <summary>
///     https://github.com/svg-net
/// </summary>
public sealed class SvgLineSegment : SvgPathSegment
{
   public SvgLineSegment(IPoint2F start,
                         IPoint2F end)
      : base(start, end)
   {
   }

   public override void AddToPath(IGraphicsPath graphicsPath)
   {
      graphicsPath.AddLine(Start, End);
   }

   public override String ToString()
   {
      return "L" + End.ToSvgString();
   }
}