using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;

namespace Gdi.Shared
{
    public class GdiGraphicsPath : GraphicsPathBase
    {
        public GdiGraphicsPath()
        {
            Path = new GraphicsPath();
        }

        public override void AddLine<TPoint>(TPoint p1, 
                                    TPoint p2) 
            //where TPoint : IPoint2D
        {
            Path.AddLine(I4(p1.X), I4(p1.Y), I4(p2.X), I4(p2.Y));
        }


        public override void LineTo<TPoint>(TPoint p1)
        {
            Path.AddLine(I4(p1.X), I4(p1.Y), I4(p1.X), I4(p1.Y));
        }

        public override void AddArc<TRectangle>(TRectangle arc, 
                                                Single startAngle, 
                                                Single endAngle)
        {
            Path.AddArc(I4(arc.X), I4(arc.Y), I4(arc.Width), I4(arc.Height), 
                startAngle, endAngle);
        }

        public override void AddBezier(Single x1,
                                       Single y1,
                                       Single x2,
                                       Single y2,
                                       Single x3,
                                       Single y3,
                                       Single x4,
                                       Single y4)
        {
            Path.AddBezier(x1, y1, x2, y2, x3, y3, x4, y4);
        }

        public override void AddBezier(IPoint2F p1,
                                       IPoint2F p2,
                                       IPoint2F p3,
                                       IPoint2F p4)
        {
            Path.AddBezier(Pf(p1), Pf(p2),Pf(p3),Pf(p4));
        }

        public override void StartFigure()
        {
            Path.StartFigure();
        }

        private static PointF Pf(IPoint2F pf) => new PointF(pf.X, pf.Y);

        public override void CloseFigure()
        {
            Path.CloseFigure();
            
        }

        public override void Dispose()
        {
            Path.Dispose();
        }

        private static Int32 I4(Double val) => Convert.ToInt32(val);

        public GraphicsPath Path { get; }
    }
}
