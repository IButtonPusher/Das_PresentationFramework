using System;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;

namespace Das.Views.Images.Svg
{
    /// <summary>
    ///     https://github.com/svg-net
    /// </summary>
    public sealed class SvgClosePathSegment : SvgPathSegment
    {
        public override void AddToPath(IGraphicsPath graphicsPath)
        {
            var pathData = graphicsPath.PathData;

            if (pathData.Points.Length > 0)
            {
                // Important for custom line caps. Force the path the close with an explicit line, not just an implicit close of the figure.
                var last = pathData.Points.Length - 1;
                if (!pathData.Points[0].Equals(pathData.Points[last]))
                {
                    var i = last;
                    while (i > 0 && pathData.Types[i] > 0) --i;
                    graphicsPath.AddLine(pathData.Points[last], pathData.Points[i]);
                }

                graphicsPath.CloseFigure();
            }
        }

        public override String ToString()
        {
            return "z";
        }
    }
}
