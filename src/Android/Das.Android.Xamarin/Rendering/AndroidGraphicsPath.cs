using System;
using Android.Graphics;
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

        private static Single R4(Double value) => Convert.ToSingle(value);

        public override void AddArc<TRectangle>(TRectangle arc, 
                                                Single startAngle, 
                                                Single endAngle)
        {

            Path.AddArc(R4(arc.Left), R4(arc.Top), R4(arc.Right), R4(arc.Bottom),
                startAngle, endAngle);
        }

        public override void CloseFigure()
        {
            Path.Close();
        }

        public Path Path { get; }
    }
}