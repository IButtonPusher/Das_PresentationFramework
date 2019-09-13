using System;
using System.Drawing;
using System.Windows.Forms;
using Das.Gdi.Core;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Rendering;

namespace Das.Gdi
{
    public class GdiRenderContext : BaseRenderContext
    {
        internal Graphics Graphics { get; set; }
        
        private readonly IPen _testPen;

        public GdiRenderContext(IMeasureContext measureContext, IViewPerspective perspective)
            : base(measureContext, perspective)
        {
            _testPen = new Views.Core.Drawing.Pen(Views.Core.Drawing.Color.Yellow, 1);
        }


        public override void DrawImage(IImage img, IRectangle rect)
        {
            var dest = GetAbsoluteGdiRectangleF(rect);
            var bmp = img.Unwrap<Bitmap>();
            Graphics.DrawImage(bmp, dest);
        }

        public override void DrawLine(IPen pen, IPoint pt1, IPoint pt2)
        {
            var usePen = TypeConverter.GetPen(pen);
            var l1 = GetAbsoluteGdiPoint(pt1);
            var l2 = GetAbsoluteGdiPoint(pt2);
            Graphics.DrawLine(usePen, l1, l2);
        }

        public override void DrawLines(IPen pen, IPoint[] points)
        {
            var usePen = TypeConverter.GetPen(pen);
            var gPoints = new PointF[points.Length];

            for (var c = 0; c < points.Length; c++)
                gPoints[c] = GetAbsoluteGdiPoint(points[c]);

            Graphics.DrawLines(usePen, gPoints);
        }

        public override void FillPie(IPoint center, double radius, double startAngle, 
            double endAngle, IBrush brush)
        {
            var c = GetAbsolutePoint(center);

            var useBrush = TypeConverter.GetBrush(brush);
            var asRect = new RectangleF((float)(c.X - radius), (float)(c.Y - radius),
                (float)radius * 2, (float)radius * 2);
            Graphics.FillPie(useBrush, asRect.X, asRect.Y, asRect.Width, asRect.Height, 
                (float)startAngle, (float)(endAngle - startAngle));
        }

        public override void DrawEllipse(IPoint center, double radius, IPen pen)
        {
            var c = GetAbsolutePoint(center);

            var usePen = TypeConverter.GetPen(pen);
            var asRect = new System.Drawing.Rectangle(Convert.ToInt32(c.X - radius),
                Convert.ToInt32(c.Y - radius),
                Convert.ToInt32(radius * 2), Convert.ToInt32(radius * 2));
            Graphics.DrawEllipse(usePen, asRect);
        }

        public override void DrawFrame(IFrame frame)
        {
            for (var c = 0; c < frame.Triangles.Count; c++)
                DrawLines(_testPen, frame.Triangles[c].PointArray);
        }

        public override void FillRect(IRectangle rect, IBrush brush)
        {
            var useRect = GetAbsoluteGdiRectangle(rect);
            var useBrush = TypeConverter.GetBrush(brush);
            Graphics.FillRectangle(useBrush, useRect);
        }

        public override void DrawRect(IRectangle rect, IPen pen)
        {
            var useRect = GetAbsoluteGdiRectangle(rect);
            var usePen = TypeConverter.GetPen(pen);
            Graphics.DrawRectangle(usePen, useRect);
        }

        public override void DrawString(string s, IFont font, IBrush brush, IPoint point)
        {
            var loc = GetAbsoluteGdiPoint(point);
            var color = TypeConverter.GetColor(brush.Color);
            var useFont = TypeConverter.GetFont(font);
            TextRenderer.DrawText(Graphics, s, useFont, loc, color);
        }

        public override void DrawString(string s, IFont font, IBrush brush, IRectangle location)
        {
            var rect = GetAbsoluteGdiRectangle(location);
            var color = TypeConverter.GetColor(brush.Color);
            var useFont = TypeConverter.GetFont(font);
            TextRenderer.DrawText(Graphics, s, useFont, rect, color);
        }

        private System.Drawing.Point GetAbsoluteGdiPoint(IPoint relativePoint)
        {
            var to = GetAbsolutePoint(relativePoint);
            return new System.Drawing.Point(Convert.ToInt32(to.X),
                Convert.ToInt32(to.Y));
        }

        private RectangleF GetAbsoluteGdiRectangleF(IRectangle relativeRect)
        {
            var absRect = GetAbsoluteRect(relativeRect);
            return TypeConverter.GetRect(absRect);
        }

        private System.Drawing.Rectangle GetAbsoluteGdiRectangle(
            IRectangle relativeRect)
        {
            var absRect = GetAbsoluteRect(relativeRect);
            return TypeConverter.GetRect(absRect);
        }
    }
}