using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Das.Gdi.Core;
using Das.Views;
using Das.Views.Colors;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Images;
using Das.Views.Rendering;
using Gdi.Shared;
using Color = Das.Views.Core.Drawing.Color;
using Pen = Das.Views.Core.Drawing.Pen;
using Rectangle = System.Drawing.Rectangle;

namespace Das.Gdi
{
    public class GdiRenderContext : BaseRenderContext
    {
        public GdiRenderContext(IViewPerspective perspective,
                                Graphics nullGraphics,
                                IVisualSurrogateProvider surrogateProvider,
                                Dictionary<IVisualElement, ValueSize> lastMeasures,
                                Dictionary<IVisualElement, ValueCube> renderPositions,
                                IThemeProvider themeProvider,
                                IVisualLineage visualLineage,
                                ILayoutQueue layoutQueue)
            : base(perspective, surrogateProvider, renderPositions,
                lastMeasures, themeProvider, visualLineage, layoutQueue)
        {
            _testPen = new Pen(Color.Yellow, 1);
            Graphics = nullGraphics;
        }

        public Graphics Graphics { get; set; }

        public override void DrawEllipse<TPoint, TPen>(TPoint center,
                                                       Double radius,
                                                       TPen pen)
        {
            var c = _boxModel.GetAbsolutePoint(center, ZoomLevel);

            var usePen = GdiTypeConverter.GetPen(pen);
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

        public override void DrawImageAt<TLocation>(IImage img,
                                                    TLocation destination)
        {
           var loc = GetAbsoluteGdiPoint(destination);
           img.UnwrapLocked<Bitmap>(b => Graphics.DrawImage(b, loc));
        }


        public override void DrawImage<TRectangle>(IImage img,
                                                   TRectangle destination)
        {
            var dest = GetAbsoluteGdiRectangleF(destination);

            img.UnwrapLocked<Bitmap>(b => Graphics.DrawImage(b, dest));
        }

        public override void DrawImage<TRectangle1, TRectangle2>(IImage img,
                                                                 TRectangle1 sourceRect,
                                                                 TRectangle2 destination)
        {
            var dest = GetAbsoluteGdiRectangleF(destination);
            var src = new Rectangle(Convert.ToInt32(sourceRect.X),
                Convert.ToInt32(sourceRect.Y),
                Convert.ToInt32(sourceRect.Width),
                Convert.ToInt32(sourceRect.Height));

            img.UnwrapLocked<Bitmap>(b => Graphics.DrawImage(b, dest, src, GraphicsUnit.Pixel));
        }

        public override void DrawLine<TPen, TPoint1, TPoint2>(TPen pen,
                                                              TPoint1 pt1,
                                                              TPoint2 pt2)
        {
            var usePen = GdiTypeConverter.GetPen(pen);
            var l1 = GetAbsoluteGdiPoint(pt1);
            var l2 = GetAbsoluteGdiPoint(pt2);
            Graphics.DrawLine(usePen, l1, l2);
        }

        public override void DrawLines(IPen pen, IPoint2D[] points)
        {
            var usePen = GdiTypeConverter.GetPen(pen);
            var gPoints = new PointF[points.Length];

            for (var c = 0; c < points.Length; c++)
                gPoints[c] = GetAbsoluteGdiPoint(points[c]);

            Graphics.DrawLines(usePen, gPoints);
        }

        public override void DrawRect<TRectangle, TPen>(TRectangle rect,
                                                        TPen pen)
        {
            var useRect = GetAbsoluteGdiRectangle(rect);
            var usePen = GdiTypeConverter.GetPen(pen);
            Graphics.DrawRectangle(usePen, useRect);
        }

        public override void DrawRoundedRect<TRectangle, TPen, TThickness>(TRectangle rect,
                                                               TPen pen,
                                                               TThickness cornerRadii)
        {
            if (cornerRadii.IsEmpty)
            {
                DrawRect(rect, pen);
                return;
            }

            var useRect = _boxModel.GetAbsoluteRect(rect, ZoomLevel);
            var usePen = GdiTypeConverter.GetPen(pen);

            using (var path = new GdiGraphicsPath())
            {
                path.SetRoundedRectangle(useRect, cornerRadii);
                Graphics.DrawPath(usePen, path.Path);
            }
        }

        public override void DrawString<TFont, TBrush, TPoint>(String s,
                                                               TFont font,
                                                               TBrush brush,
                                                               TPoint location)
        {
            var loc = GetAbsoluteGdiPoint(location);

            if (!(brush is SolidColorBrush scb))
                throw new NotSupportedException(nameof(DrawString) +
                                                " - " + brush);

            Debug.WriteLine("draw string " + s + " at " + location + " brush " + brush);

            var color = GdiTypeConverter.GetColor(scb.Color);
            var useFont = GdiTypeConverter.GetFont(font);
            TextRenderer.DrawText(Graphics, s, useFont, loc, color,
                TextFormatFlags.PreserveGraphicsClipping);
        }

        public override void DrawString<TFont, TBrush, TRectangle>(String s,
                                                                   TFont font,
                                                                   TRectangle location,
                                                                   TBrush brush)
        {
            var rect = GetAbsoluteGdiRectangle(location);

            var useBrush = GdiTypeConverter.GetBrush(brush);
            var useFont = GdiTypeConverter.GetFont(font);
            Graphics.DrawString(s, useFont, useBrush, rect, StringFormat.GenericDefault);
            // text renderer doesn't wrap the text
            //TextRenderer.DrawText(Graphics, s, useFont, rect, color);
        }

        public override void FillPie<TPoint, TBrush>(TPoint center,
                                                     Double radius,
                                                     Double startAngle,
                                                     Double endAngle,
                                                     TBrush brush)
        {
            var c = _boxModel.GetAbsolutePoint(center, ZoomLevel);

            var useBrush = GdiTypeConverter.GetBrush(brush);
            var asRect = new RectangleF((Single) (c.X - radius), (Single) (c.Y - radius),
                (Single) radius * 2, (Single) radius * 2);

            if (asRect.Width <= 0 || asRect.Height <= 0)
                return;

            Graphics.FillPie(useBrush, asRect.X, asRect.Y, asRect.Width, asRect.Height,
                (Single) startAngle, (Single) (endAngle - startAngle));
        }

        public override void FillRectangle<TRectangle, TBrush>(TRectangle rect,
                                                               TBrush brush)
        {
            var useRect = GetAbsoluteGdiRectangle(rect);
            var useBrush = GdiTypeConverter.GetBrush(brush);

      Debug.WriteLine("fill rect " + useRect + " with " + brush);

            Graphics.FillRectangle(useBrush, useRect);
        }

        public override void FillRoundedRectangle<TRectangle, TBrush, TThickness>(TRectangle rect,
                                                                      TBrush brush,
                                                                      TThickness cornerRadii)
        {
            if (cornerRadii.IsEmpty)
            {
                FillRectangle(rect, brush);
                return;
            }

            var useRect = _boxModel.GetAbsoluteRect(rect, ZoomLevel);
            
            var useBrush = GdiTypeConverter.GetBrush(brush);

            using (var path = new GdiGraphicsPath())
            {
                path.SetRoundedRectangle(useRect, cornerRadii);
                Graphics.FillPath(useBrush, path.Path);
            }
        }

        protected override ValueRectangle GetCurrentClip()
        {
            if (_clipCounter == 0)
                return ValueRectangle.Empty;
            var clip = Graphics.ClipBounds;
            if (clip == null)
                return ValueRectangle.Empty;
            if (clip.Width == 0 && clip.Height == 0)
                return ValueRectangle.Empty;

            return new ValueRectangle(clip.Left, clip.Top,
                clip.Width, clip.Height);
        }

        protected override void PopClip<TRectangle>(TRectangle rect)
        {
            _clipCounter--;

            if (_clipCounter == 0)
                Graphics.ResetClip();
            else
            {
                var useRect = GetAbsoluteGdiRectangle(rect);
                Graphics.ExcludeClip(useRect);
            }
        }

        protected override void PushClip<TRectangle>(TRectangle rect)
        {
            _clipCounter++;
            var useRect = GdiTypeConverter.GetRect(rect);
            Graphics.SetClip(useRect);
        }

        private Point GetAbsoluteGdiPoint(IPoint2D relativePoint2D)
        {
            var to = _boxModel.GetAbsolutePoint(relativePoint2D, ZoomLevel);
            return new Point(Convert.ToInt32(to.X),
                Convert.ToInt32(to.Y));
        }

        private Rectangle GetAbsoluteGdiRectangle(
            IRectangle relativeRect)
        {
            var absRect = _boxModel.GetAbsoluteRect(relativeRect, ZoomLevel);
            return GdiTypeConverter.GetRect(absRect);
        }

        private RectangleF GetAbsoluteGdiRectangleF<TRectangle>(TRectangle relativeRect)
            where TRectangle : IRectangle
        {
            var absRect = _boxModel.GetAbsoluteRect(relativeRect, ZoomLevel);
            return GdiTypeConverter.GetRect(absRect);
        }

        private readonly IPen _testPen;
        private Int32 _clipCounter;
    }
}