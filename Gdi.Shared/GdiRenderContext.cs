using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Das.Gdi.Core;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Rendering;
using Gdi.Shared;
using Color = Das.Views.Core.Drawing.Color;
using Pen = Das.Views.Core.Drawing.Pen;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace Das.Gdi
{
    public class GdiRenderContext : BaseRenderContext
    {
        public GdiRenderContext(IMeasureContext measureContext, 
                                IViewPerspective perspective,
                                Graphics nullGraphics)
            : base(measureContext, perspective)
        {
            _testPen = new Pen(Color.Yellow, 1);
            Graphics = nullGraphics;
        }

        internal Graphics Graphics { get; set; }

        public override void DrawEllipse(IPoint2D center, Double radius, IPen pen)
        {
            var c = GetAbsolutePoint(center);

            var usePen = TypeConverter.GetPen(pen);
            var asRect = new Rectangle(Convert.ToInt32(c.X - radius),
                Convert.ToInt32(c.Y - radius),
                Convert.ToInt32(radius * 2), Convert.ToInt32(radius * 2));
            Graphics.DrawEllipse(usePen, asRect);
        }

        public override void DrawFrame(IFrame frame)
        {
            for (var c = 0; c < frame.Triangles.Count; c++)
                DrawLines(_testPen, frame.Triangles[c].PointArray);
        }


        public override void DrawImage(IImage img, IRectangle rect)
        {
            var dest = GetAbsoluteGdiRectangleF(rect);
            var bmp = img.Unwrap<Bitmap>();
            Graphics.DrawImage(bmp, dest);
        }

        public override IImage? GetImage(Stream stream)
        {
            var bmp = new Bitmap(stream);
            return new GdiBitmap(bmp);
        }

        public override void DrawLine(IPen pen, IPoint2D pt1, IPoint2D pt2)
        {
            var usePen = TypeConverter.GetPen(pen);
            var l1 = GetAbsoluteGdiPoint(pt1);
            var l2 = GetAbsoluteGdiPoint(pt2);
            Graphics.DrawLine(usePen, l1, l2);
        }

        public override void DrawLines(IPen pen, IPoint2D[] points)
        {
            var usePen = TypeConverter.GetPen(pen);
            var gPoints = new PointF[points.Length];

            for (var c = 0; c < points.Length; c++)
                gPoints[c] = GetAbsoluteGdiPoint(points[c]);

            Graphics.DrawLines(usePen, gPoints);
        }

        public override void DrawRect(IRectangle rect, IPen pen)
        {
            var useRect = GetAbsoluteGdiRectangle(rect);
            var usePen = TypeConverter.GetPen(pen);
            Graphics.DrawRectangle(usePen, useRect);
        }

        public override void DrawString(String s, IFont font, IBrush brush, IPoint2D point2D)
        {
            var loc = GetAbsoluteGdiPoint(point2D);

            if (!(brush is SolidColorBrush scb))
                throw new NotImplementedException();

            var color = TypeConverter.GetColor(scb.Color);
            var useFont = TypeConverter.GetFont(font);
            TextRenderer.DrawText(Graphics, s, useFont, loc, color);
        }

        public override void DrawString(String s, IFont font, IBrush brush, IRectangle location)
        {
            var rect = GetAbsoluteGdiRectangle(location);
            
            var useBrush = TypeConverter.GetBrush(brush);
            var useFont = TypeConverter.GetFont(font);
            Graphics.DrawString(s, useFont, useBrush, rect, StringFormat.GenericDefault);
            // text renderer doesn't wrap the text
            //TextRenderer.DrawText(Graphics, s, useFont, rect, color);
        }

        public override void FillPie(IPoint2D center, 
                                     Double radius, 
                                     Double startAngle,
                                     Double endAngle, IBrush brush)
        {
            var c = GetAbsolutePoint(center);

            var useBrush = TypeConverter.GetBrush(brush);
            var asRect = new RectangleF((Single) (c.X - radius), (Single) (c.Y - radius),
                (Single) radius * 2, (Single) radius * 2);
            Graphics.FillPie(useBrush, asRect.X, asRect.Y, asRect.Width, asRect.Height,
                (Single) startAngle, (Single) (endAngle - startAngle));
        }

        public override void FillRect(IRectangle rect, IBrush brush)
        {
            var useRect = GetAbsoluteGdiRectangle(rect);
            var useBrush = TypeConverter.GetBrush(brush);
            Graphics.FillRectangle(useBrush, useRect);
        }

        private Point GetAbsoluteGdiPoint(IPoint2D relativePoint2D)
        {
            var to = GetAbsolutePoint(relativePoint2D);
            return new Point(Convert.ToInt32(to.X),
                Convert.ToInt32(to.Y));
        }

        private Rectangle GetAbsoluteGdiRectangle(
            IRectangle relativeRect)
        {
            var absRect = GetAbsoluteRect(relativeRect);
            return TypeConverter.GetRect(absRect);
        }

        private RectangleF GetAbsoluteGdiRectangleF(IRectangle relativeRect)
        {
            var absRect = GetAbsoluteRect(relativeRect);
            return TypeConverter.GetRect(absRect);
        }

        private readonly IPen _testPen;
    }
}