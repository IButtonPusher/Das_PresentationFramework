using System;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;

namespace Das.Views.Rendering
{
    public abstract class GraphicsPathBase : IGraphicsPath
    {
        public abstract void Dispose();

        public abstract void LineTo<TPoint>(TPoint p1) where TPoint : IPoint2D;

        public abstract void AddLine<TPoint>(TPoint p1,
                                             TPoint p2) where TPoint : IPoint2F;

        public abstract void AddArc<TRectangle>(TRectangle arc,
                                                Single startAngle,
                                                Single endAngle)
            where TRectangle : IRectangle;

        public abstract void AddBezier(Single x1,
                                       Single y1,
                                       Single x2,
                                       Single y2,
                                       Single x3,
                                       Single y3,
                                       Single x4,
                                       Single y4);

        public abstract void AddBezier(IPoint2F p1,
                                       IPoint2F p2,
                                       IPoint2F p3,
                                       IPoint2F p4);

        public abstract void StartFigure();

        public abstract void CloseFigure();

        // https://github.com/koszeggy/KGySoft.Drawing/blob/625e71e38a0d2e2c94821629f34e797ceae4015c/KGySoft.Drawing/Drawing/_Extensions/GraphicsExtensions.cs#L287
        public void SetRoundedRectangle<TThickness, TRect>(TRect bounds,
                                                           TThickness cornerRadii)
            where TThickness : IThickness where TRect : IRectangle
        {
            var radiusTopLeft = Convert.ToInt32(cornerRadii.Left);
            var radiusTopRight = Convert.ToInt32(cornerRadii.Top);
            var radiusBottomRight = Convert.ToInt32(cornerRadii.Right);
            var radiusBottomLeft = Convert.ToInt32(cornerRadii.Bottom);

            var size = new Size(radiusTopLeft << 1, radiusTopLeft << 1);
            var arc = new Rectangle(bounds.Location, size);

            // top left arc
            if (radiusTopLeft == 0)
                LineTo(arc.Location);
            else
                AddArc(arc, 180, 90);

            // top right arc
            if (radiusTopRight != radiusTopLeft)
            {
                size = new Size(radiusTopRight << 1, radiusTopRight << 1);
                arc.Size = size;
            }

            arc.X = bounds.Right - size.Width;
            if (radiusTopRight == 0)
                LineTo(arc.Location);
            else
                AddArc(arc, 270, 90);

            // bottom right arc
            if (radiusTopRight != radiusBottomRight)
            {
                size = new Size(radiusBottomRight << 1, radiusBottomRight << 1);
                arc.X = bounds.Right - size.Width;
                arc.Size = size;
            }

            arc.Y = bounds.Bottom - size.Height;
            if (radiusBottomRight == 0)
                LineTo(arc.Location);
            else
                AddArc(arc, 0, 90);

            // bottom left arc
            if (radiusBottomRight != radiusBottomLeft)
            {
                arc.Size = new Size(radiusBottomLeft << 1, radiusBottomLeft << 1);
                arc.Y = bounds.Bottom - arc.Height;
            }

            arc.X = bounds.Left;
            if (radiusBottomLeft == 0)
                LineTo(arc.Location);
            else
                AddArc(arc, 90, 90);

            CloseFigure();
        }

        public IPathData PathData => throw new NotImplementedException();

        protected static Single R4(Double val) => Convert.ToSingle(val);
    }
}
