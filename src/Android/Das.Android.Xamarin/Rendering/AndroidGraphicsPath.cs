using System;
using System.Collections.Generic;
using System.Linq;
using Android.Graphics;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Images;
using Das.Views.Rendering;

namespace Das.Xamarin.Android.Rendering
{
    public class AndroidGraphicsPath : GraphicsPathBase
    {
        public AndroidGraphicsPath()
        {
            Path = new Path();
            _pointTypes = new List<PathPointType>();
        }

        public override void Dispose()
        {
            
        }

        public override void LineTo<TPoint>(TPoint p1) 
        {
            Path.LineTo(R4(p1.X), R4(p1.Y));
            _pointTypes.Add(PathPointType.Line);
        }

        public override void AddLine<TPoint>(TPoint p1,
                                             TPoint p2)
        {
            Path.MoveTo(p1.X, p1.Y);
            Path.LineTo(p2.X, p2.Y);
            _pointTypes.Add(PathPointType.Line);
        }

        public override void AddArc<TRectangle>(TRectangle arc, 
                                                Single startAngle, 
                                                Single endAngle)
        {
            Path.AddArc(R4(arc.Left), R4(arc.Top), R4(arc.Right), R4(arc.Bottom),
                startAngle, endAngle);

            _pointTypes.Add(PathPointType.Bezier);
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
            Path.MoveTo(x1, y1);
            Path.CubicTo(x2, y2, x3, y3, x4, y4);
            _pointTypes.Add(PathPointType.Bezier3);
        }

        public override void AddBezier(IPoint2F p1,
                                       IPoint2F p2,
                                       IPoint2F p3,
                                       IPoint2F p4)
        {
            Path.MoveTo(p1.X, p1.Y);
            Path.CubicTo(p2.X, p2.Y, p3.X, p3.Y, p4.X, p4.Y);
            _pointTypes.Add(PathPointType.Bezier3);
        }

        public override void StartFigure()
        {
            Path.Reset();
            _pointTypes.Add(PathPointType.Start);
        }

        public override void CloseFigure()
        {
            Path.Close();
            _pointTypes.Add(PathPointType.CloseSubpath);
        }

        public override ValueSize Size
        {
            get
            {
                var pm = new PathMeasure(Path, false);
                return new ValueSize(pm.Length, pm.Length);
            }
        }

        public override IPathData PathData
        {
            get
            {
                //var pointArray = new ValuePoint2F[20];
                var pointArray = new List<IPoint2F>(20);

                var pm = new PathMeasure(Path, false);
                var length = pm.Length;
                var distance = 0f;
                var speed = length / 20;
                var counter = 0;
                var aCoordinates = new Single[2];

                while ((distance < length) && (counter < 20)) {
                    // get point from the path
                    pm.GetPosTan(distance, aCoordinates, null);
                    //pointArray[counter] = new ValuePoint2F(aCoordinates[0],
                    //    aCoordinates[1]);
                    pointArray.Add(new ValuePoint2F(aCoordinates[0],
                        aCoordinates[1]));
                    counter++;
                    distance = distance + speed;
                }

                //IEnumerable<IPoint2F> rdrr = pointArray;

                return new PathData(pointArray, _pointTypes.Cast<Byte>());
            }
        }

        public override T Unwrap<T>()
        {
            if (Path is T good)
                return good;

            throw new InvalidCastException();
        }

        public override IImage ToImage(Int32 width,
                                       Int32 height,
                                       IColor? stroke,
                                       IBrush? fill)
        { var androidPath = Path;

            var bmp = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888)
                      ?? throw new InvalidOperationException();

            var canvas = new Canvas(bmp);
            using (var paint = new Paint())
            {
                if (fill is { } set)
                {
                    paint.SetBackgroundColor(set);
                }

                if (stroke is { } different)
                {
                    paint.SetStrokeColor(different);
                }

                canvas.DrawPath(androidPath, paint);
            }

            return new AndroidBitmap(bmp, null);
        }

        public Path Path { get; }

        private readonly List<PathPointType> _pointTypes;
    }
}