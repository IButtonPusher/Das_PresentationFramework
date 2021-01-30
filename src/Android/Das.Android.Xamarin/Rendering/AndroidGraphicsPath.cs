using System;
using Android.Graphics;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;

namespace Das.Xamarin.Android.Rendering
{
    public class AndroidGraphicsPath : GraphicsPathBase
    {
        public AndroidGraphicsPath()
        {
            Path = new Path();
        }

        public override void Dispose()
        {
            
        }

        public override void LineTo<TPoint>(TPoint p1) 
        {
            Path.LineTo(R4(p1.X), R4(p1.Y));
        }

        public override void AddLine<TPoint>(TPoint p1,
                                             TPoint p2)
        {
            throw new NotImplementedException();
        }

        public override void AddArc<TRectangle>(TRectangle arc, 
                                                Single startAngle, 
                                                Single endAngle)
        {
            Path.AddArc(R4(arc.Left), R4(arc.Top), R4(arc.Right), R4(arc.Bottom),
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
            throw new NotImplementedException();
        }

        public override void AddBezier(IPoint2F p1,
                                       IPoint2F p2,
                                       IPoint2F p3,
                                       IPoint2F p4)
        {
            throw new NotImplementedException();
        }

        public override void StartFigure()
        {
            Path.Reset();
        }

        public override void CloseFigure()
        {
            Path.Close();
        }

        public Path Path { get; }
    }
}