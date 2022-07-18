using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Images;
using Das.Views.Rendering;
using Das.Views.Transforms;

namespace OpenGLTests
{
   class TestGraphicsPath: GraphicsPathBase
    {
        public TestGraphicsPath()
        {
            Path = new GraphicsPath();
            
            //Path.Transform();
        }

        public override void AddLine<TPoint>(TPoint p1,
                                             TPoint p2)
        {
            Path.AddLine(R4(p1.X), R4(p1.Y), R4(p2.X), R4(p2.Y));
        }

        public override void LineTo<TPoint>(TPoint p1)
        {
            Path.AddLine(R4(p1.X), R4(p1.Y), R4(p1.X), R4(p1.Y));
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

        private static PointF Pf(IPoint2F pf) => new(pf.X, pf.Y);

        public override void CloseFigure()
        {
            Path.CloseFigure();
            
        }

        public override void Transform(TransformationMatrix matrix)
        {
            var gdiMatrix = new Matrix(Convert.ToSingle(matrix.ScaleX),
                Convert.ToSingle(matrix.SkewX),
                Convert.ToSingle(matrix.SkewY),
                Convert.ToSingle(matrix.ScaleY),
                Convert.ToSingle(matrix.OffsetX),
                Convert.ToSingle(matrix.OffsetY));
                
            Path.Transform(gdiMatrix);
        }

        public override ValueSize Size
        {
            get
            {
                var b = Path.GetBounds();
                return new ValueSize(b.Width, b.Height);
            }
        }

        public override IPathData PathData
        {
            get
            {
                //for (var c = 0; c < Path.PathData.Points.Length; c++)
                //{
                //    System.Diagnostics.Debug.WriteLine(c + "\t" + Path.PathData.Points[c] + "\t"
                //                                       + (Int32)Path.PathData.Types[c]);
                //}

                var res = new Das.Views.Rendering.PathData(
                    GetLocalPoints(Path.PathData.Points),
                    Path.PathData.Types);

                return res;
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
        {
            var gdiPath = Path;
            
            var bmp = new Bitmap(width, height);

            using (var g = bmp.GetSmoothGraphics())
            {
                if (fill is { } set)
                {
                    var brush = TestTypeConverter.GetBrush(set);
                    g.FillPath(brush, gdiPath);
                }

                if (stroke is { } different)
                {
                    var p = new Das.Views.Core.Drawing.Pen(different, 1);
                    //using (var usePen = GdiTypeConverter.GetPen(p))
                    var usePen = TestTypeConverter.GetPen(p);
                    {
                        g.DrawPath(usePen, gdiPath);
                    }
                }
            }

            return new TestBitmap(bmp, null);
        }

        private static IEnumerable<IPoint2F> GetLocalPoints(IEnumerable<PointF> points)
        {
            foreach (var p in points)
                yield return new ValuePoint2F(p.X, p.Y);
        }

        public override void Dispose()
        {
            Path.Dispose();
        }

        private static Int32 I4(Double val) => Convert.ToInt32(val);

        public GraphicsPath Path { get; }
    }
}
