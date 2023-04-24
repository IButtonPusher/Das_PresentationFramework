using System;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;

namespace Das.Views.Images.Svg;

/// <summary>
///     https://github.com/svg-net
/// </summary>
public sealed class SvgClosePathSegment : SvgPathSegment
{
   public override void AddToPath(IGraphicsPath graphicsPath)
   {
      var pathData = graphicsPath.PathData;
      var points = pathData.Points;

      if (points.Length <= 0)
         return;

      // Important for custom line caps. Force the path the close with an explicit line, not just an implicit close of the figure.
      var last = points.Length - 1;
      if (!points[0].Equals(points[last]))
      {
         var i = last;
         while (i > 0 && pathData.Types[i] > 0) --i;
         graphicsPath.AddLine(points[last], points[i]);
      }

      graphicsPath.CloseFigure();
   }

   public override String ToString()
   {
      return "z";
   }
}