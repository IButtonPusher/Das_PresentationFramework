using System;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;

namespace Das.Views.Images.Svg
{
    /// <summary>
    ///     https://github.com/svg-net
    /// </summary>
    public class SvgMoveToSegment : SvgPathSegment
    {
        public SvgMoveToSegment(IPoint2F moveTo)
            : base(moveTo, moveTo)
        {
        }

        public override void AddToPath(IGraphicsPath graphicsPath)
        {
            graphicsPath.StartFigure();
        }

        public override String ToString()
        {
            return "M" + Start.ToSvgString();
        }
    }
}
