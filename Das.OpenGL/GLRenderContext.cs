using Das.Views.Rendering;
using System;
using System.Diagnostics;
using System.IO;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;

namespace Das.OpenGL
{
    public class GLRenderContext : BaseRenderContext
    {
        public GLRenderContext(IViewPerspective perspective,
                               IGLContext openGlContext,
                               IFontProvider fontProvider,
                               IVisualSurrogateProvider surrogateProvider)
            : base(perspective, surrogateProvider)
        {
            _openGlContext = openGlContext;
            _fontProvider = fontProvider;
        }

        private readonly IGLContext _openGlContext;
        private readonly IFontProvider _fontProvider;
        private const Double TwoPi = 2.0 * Math.PI;

        public override void DrawString<TFont, TBrush, TPoint>(String s,
                                                               TFont font,
                                                               TBrush brush,
                                                               TPoint location)
        {
            var to = GetAbsolutePoint(location);
            var renderer = _fontProvider.GetRenderer(font);
            renderer.DrawString(s, brush, to);
        }

        public override void DrawRoundedRect<TRectangle, TPen>(TRectangle rect,
                                                               TPen pen,
                                                               Double cornerRadius)
        {
            throw new NotImplementedException();
        }

        public override void FillRectangle<TRectangle, TBrush>(TRectangle orect,
                                                               TBrush brush)
        {
            var rect = orect;
            SetOrtho();

            GL.glMatrixMode(GL.MODELVIEW);
            GL.glLoadIdentity();
            if (brush is SolidColorBrush scb)
                SetColor(scb.Color);

            GL.glRectd(rect.Left, rect.Top, rect.Right, rect.Bottom);

            GL.glFlush();
        }

        public override void FillRoundedRectangle<TRectangle, TBrush>(TRectangle rect,
                                                                      TBrush brush,
                                                                      Double cornerRadius)
        {
            throw new NotImplementedException();
        }

        private static void SetColor(IColor color)
        {
            var r = (Single)color.R / 255;
            var g = (Single)color.G / 255;
            var b = (Single)color.B / 255;

            GL.glColor3f(r, g, b);
        }

        public override void DrawString<TFont, TBrush, TRectangle>(String s,
                                                                   TFont font,
                                                                   TRectangle location,
                                                                   TBrush brush)
        {
            throw new NotImplementedException();
        }

        public override  void DrawImage<TRectangle>(IImage img, 
                                                    TRectangle destination)
        {
            throw new NotImplementedException();
        }

        public override IImage GetNullImage()
        {
            throw new NotImplementedException();
        }

        public override void DrawImage<TRectangle1, TRectangle2>(IImage img,
                                                                 TRectangle1 sourceRect,
                                                                 TRectangle2 destination)
        {
            throw new NotImplementedException();
        }

        public override IImage? GetImage(Stream stream)
        {
            throw new NotImplementedException();
        }

        public override void DrawLine<TPen, TPoint1, TPoint2>(TPen pen,
                                                              TPoint1 pt1,
                                                              TPoint2 pt2)
        {
            SetOrtho();

            GL.glMatrixMode(GL.MODELVIEW);
            GL.glLoadIdentity();

            GL.glBegin(GL.LINES);
            SetColor(pen.Color);


            GL.glVertex2d(pt1.X, pt1.Y);
            GL.glVertex2d(pt2.X, pt2.Y);

            GL.glEnd();
            GL.glFlush(); 
        }

        public override void DrawLines(IPen pen, IPoint2D[] points)
        {
            SetOrtho();

            GL.glBegin(GL.LINE_LOOP);
            SetColor(pen.Color);
            
            for (var d = 0; d < points.Length; d++)
                GL.glVertex2d(points[d].X, points[d].Y);

            GL.glEnd();
            GL.glFlush();
        }

        public override void DrawRect<TRectangle, TPen>(TRectangle rect,
                                                        TPen pen)
        {
            var points = new IPoint2D[] { rect.TopLeft, rect.TopRight,
                rect.BottomRight, rect.BottomLeft };

            DrawLines(pen, points);
        }

        private void SetOrtho()
        {
            GL.glMatrixMode(GL.PROJECTION);
            GL.glLoadIdentity();

            var left = 0 - CurrentLocation.X;
            var right = _openGlContext.Size.Width - CurrentLocation.X;

            var top = 0 - CurrentLocation.Y;
            var bottom = _openGlContext.Size.Height - CurrentLocation.Y;

            GL.glOrtho(left, right, bottom, top, -1, 1);
        }

        public override void FillPie<TPoint, TBrush>(TPoint center,
                                                     Double radius,
                                                     Double startAngle,
                                                     Double endAngle,
                                                     TBrush brush)
        {
            startAngle += 90;
            endAngle += 90;

            var s = Math.Min(startAngle, endAngle);
            var e = Math.Max(startAngle, endAngle);
            startAngle = s;
            endAngle = e;

            var startPct = startAngle * 100 / 360.0;
            var endPct = endAngle * 100 / 360.0;

            SetOrtho();

            GL.glMatrixMode(GL.MODELVIEW);
            GL.glLoadIdentity();

            GL.glBegin(GL.TRIANGLE_FAN);
            if (brush is SolidColorBrush scb)
                SetColor(scb.Color);

            var fx = (Single)center.X;
            var fy = (Single)center.Y;

            GL.glVertex3f(fx, fy, 0);

            for (var j = startPct; j <= endPct; j += 1)
                DrawSemiSlice(fx, fy, radius, j);
            DrawSemiSlice(fx, fy, radius, endPct);

            GL.glEnd();
        }

        public override void DrawEllipse<TPoint, TPen>(TPoint center,
                                                       Double radius,
                                                       TPen pen)
        {
            const Single halfPie = (Single)(Math.PI / 180f);

            SetOrtho();

            SetColor(pen.Color);
            GL.glLineWidth(pen.Thickness);
            
            GL.glBegin(GL.POINTS);
          
            GL.glEnd();

            GL.glBegin(GL.LINE_LOOP);
            
            for (var i = 0f; i < 360f; i++)
            {
                var x = (Single)((Math.Cos(i * halfPie) * radius) + center.X);
                var y = (Single)((Math.Sin(i * halfPie) * radius) + center.Y);
                GL.glVertex2f(x, y);
            }

            GL.glEnd();
            GL.glPopAttrib();
        }

        private static void DrawSemiSlice(Double x, Double y, Double radius, Double j)
        {
            var t = (TwoPi * j / 100.0) + Math.PI;

            var xVert = (Single)(x - Math.Sin(t) * radius);
            var yVert = (Single)(y + Math.Cos(t) * radius);

            GL.glVertex3f(xVert, yVert, 0);
        }

        private Int32 _wotCount;
        private static readonly Stopwatch _sw = new Stopwatch();

        public override void DrawFrame(IFrame frame)
        {
            _sw.Restart();

            SetOrtho();

            GL.glMatrixMode(GL.MODELVIEW);
            GL.glLoadIdentity();

            for (var c = 0; c < frame.Triangles.Count; c++)
            {
                GL.glBegin(GL.LINE_LOOP);
                GL.glColor3f(1.0f, 0.5f, 0.0f);

                //GL.glVertex3f(frame.Triangles[c]);

                var points = frame.Triangles[c].PointArray;
                for (var d = 0; d < points.Length; d++)
                {
                    GL.glVertex2d(points[d].X, points[d].Y);
                    _wotCount++;
                }

                GL.glEnd();
                GL.glFlush();
            }

            Debug.WriteLine("cnt: " + _wotCount + " " + _sw.ElapsedMilliseconds);
        }
    }
}
