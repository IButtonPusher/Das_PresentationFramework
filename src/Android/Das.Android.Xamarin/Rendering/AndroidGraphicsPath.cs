using System;
using Android.Graphics;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;

namespace Das.Xamarin.Android.Rendering
{
    public class AndroidGraphicsPath : IGraphicsPath
    {
        public AndroidGraphicsPath()
        {
            Path = new Path();
        }

        public void Dispose()
        {
            
        }

        public void LineTo<TPoint>(TPoint p1) where TPoint : IPoint2D
        {
            Path.LineTo(R4(p1.X), R4(p1.Y));
        }

        private static Single R4(Double value) => Convert.ToSingle(value);

        public void AddArc<TRectangle>(TRectangle arc, 
                                       Single startAngle, 
                                       Single endAngle) where TRectangle : IRectangle
        {

            Path.AddArc(R4(arc.Left), R4(arc.Top), R4(arc.Right), R4(arc.Bottom),
                startAngle, endAngle);
        }

        public void CloseFigure()
        {
            Path.Close();
        }

        public Path Path { get; }
    }
}