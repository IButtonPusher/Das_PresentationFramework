using System;
using System.Drawing.Drawing2D;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;

namespace Gdi.Shared
{
    public class GdiGraphicsPath : IGraphicsPath
    {
        public GdiGraphicsPath()
        {
            Path = new GraphicsPath();
        }

        public void AddLine<TPoint>(TPoint p1, 
                                    TPoint p2) 
            where TPoint : IPoint2D
        {
            Path.AddLine(I4(p1.X), I4(p1.Y), I4(p2.X), I4(p2.Y));
        }


        public void LineTo<TPoint>(TPoint p1) 
            where TPoint : IPoint2D
        {
            Path.AddLine(I4(p1.X), I4(p1.Y), I4(p1.X), I4(p1.Y));
        }

        public void AddArc<TRectangle>(TRectangle arc, 
                                       Single startAngle, 
                                       Single endAngle) 
            where TRectangle : IRectangle
        {
            Path.AddArc(I4(arc.X), I4(arc.Y), I4(arc.Width), I4(arc.Height), 
                startAngle, endAngle);
        }

        public void CloseFigure()
        {
            Path.CloseFigure();
            
        }

        public void Dispose()
        {
            Path.Dispose();
        }

        private static Int32 I4(Double val) => Convert.ToInt32(val);

        public GraphicsPath Path { get; }
    }
}
