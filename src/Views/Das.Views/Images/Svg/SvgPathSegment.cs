using System;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;

namespace Das.Views.Images.Svg
{
    /// <summary>
    ///     https://github.com/svg-net
    /// </summary>
    public abstract class SvgPathSegment
    {
        protected SvgPathSegment()
        {
            Start = ValuePoint2F.Empty;
            End = ValuePoint2F.Empty;
        }

        protected SvgPathSegment(IPoint2F start,
                                 IPoint2F end)
        {
            Start = start;
            End = end;
        }

        public IPoint2F End { get; set; }

        public IPoint2F Start { get; set; }

        public abstract void AddToPath(IGraphicsPath graphicsPath);

        public SvgPathSegment Clone()
        {
            return MemberwiseClone() as SvgPathSegment ?? throw new Exception();
        }
    }
}
