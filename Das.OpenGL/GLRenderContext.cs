using Das.Views.Rendering;
using System;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;

namespace Das.OpenGL
{
    public class GLRenderContext : BaseRenderContext
    {
        public GLRenderContext(IMeasureContext measureContext, IViewPerspective perspective,
            IGLContext openGlContext, IFontProvider fontProvider) 
            : base(measureContext, perspective)
        {
            _openGlContext = openGlContext;
            _fontProvider = fontProvider;
        }

        private readonly IGLContext _openGlContext;
        private readonly IFontProvider _fontProvider;
        private const Double TwoPi = 2.0 * Math.PI;

        public override void DrawString(string s, IFont font, IBrush brush, IPoint point)
        {
            var to = GetAbsolutePoint(point);
            var renderer = _fontProvider.GetRenderer(font);
            renderer.DrawString(s, brush, to);
        }

        public override void FillRect(IRectangle orect, IBrush brush)
        {
            var rect = orect;
            SetOrtho();

            GL.glMatrixMode(GL.MODELVIEW);
            GL.glLoadIdentity();
            SetColor(brush.Color);

            GL.glRectd(rect.Left, rect.Top, rect.Right, rect.Bottom);

            GL.glFlush();
        }

        private static void SetColor(IColor color)
        {
            var r = (float)color.R / 255;
            var g = (float)color.G / 255;
            var b = (float)color.B / 255;

            GL.glColor3f(r, g, b);
        }

        public override void DrawString(string s, IFont font, IBrush brush, IRectangle location)
        {
            throw new NotImplementedException();
        }

        public override void DrawImage(IImage img, IRectangle rect)
        {
            throw new NotImplementedException();
        }

        public override void DrawLine(IPen pen, IPoint pt1, IPoint pt2)
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

        public override void DrawLines(IPen pen, IPoint[] points)
        {
            SetOrtho();

            GL.glBegin(GL.LINE_LOOP);
            SetColor(pen.Color);
            
            for (var d = 0; d < points.Length; d++)
                GL.glVertex2d(points[d].X, points[d].Y);

            GL.glEnd();
            GL.glFlush();
        }

        public override void DrawRect(IRectangle rect, IPen pen)
        {
            var points = new IPoint[] { rect.TopLeft, rect.TopRight,
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

        public override void FillPie(IPoint center, double radius, double startAngle, 
            double endAngle, IBrush brush)
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
            SetColor(brush.Color);

            var fx = (float)center.X;
            var fy = (float)center.Y;

            GL.glVertex3f(fx, fy, 0);

            for (var j = startPct; j <= endPct; j += 1)
                DrawSemiSlice(fx, fy, radius, j);
            DrawSemiSlice(fx, fy, radius, endPct);

            GL.glEnd();
        }

        public override void DrawEllipse(IPoint center, double radius, IPen pen)
        {
            const float halfPie = (float)(Math.PI / 180f);

            SetOrtho();

            SetColor(pen.Color);
            GL.glLineWidth(pen.Thickness);
            
            GL.glBegin(GL.POINTS);
          
            GL.glEnd();

            GL.glBegin(GL.LINE_LOOP);
            
            for (var i = 0f; i < 360f; i++)
            {
                var x = (float)((Math.Cos(i * halfPie) * radius) + center.X);
                var y = (float)((Math.Sin(i * halfPie) * radius) + center.Y);
                GL.glVertex2f(x, y);
            }

            GL.glEnd();
            GL.glPopAttrib();
        }

        private static void DrawSemiSlice(Double x, Double y, Double radius, Double j)
        {
            var t = (TwoPi * j / 100.0) + Math.PI;

            var xVert = (float)(x - Math.Sin(t) * radius);
            var yVert = (float)(y + Math.Cos(t) * radius);

            GL.glVertex3f(xVert, yVert, 0);
        }

        public override void DrawFrame(IFrame frame)
        {
            SetOrtho();

            GL.glMatrixMode(GL.MODELVIEW);
            GL.glLoadIdentity();

            for (var c = 0; c < frame.Triangles.Count; c++)
            {
                GL.glBegin(GL.LINE_LOOP);
                GL.glColor3f(1.0f, 0.5f, 0.0f);

                var points = frame.Triangles[c].PointArray;
                for (var d = 0; d < points.Length; d++)
                    GL.glVertex2d(points[d].X, points[d].Y);

                GL.glEnd();
                GL.glFlush();
            }
        }
    }
}
