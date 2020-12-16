using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Das.Gdi.Core;
using Das.Views;
using Das.Views.Controls;
using Das.Views.Core;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;
using Das.Views.Styles;
using Color = Das.Views.Core.Drawing.Color;
using Pen = Das.Views.Core.Drawing.Pen;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace Das.Gdi
{
    public class GdiRenderContext : BaseRenderContext
    {
        private readonly IImageProvider _imageProvider;

        public GdiRenderContext(IViewPerspective perspective,
                                Graphics nullGraphics,
                                IVisualSurrogateProvider surrogateProvider,
                                IImageProvider imageProvider,
                                Dictionary<IVisualElement, ValueSize> lastMeasures,
                                Dictionary<IVisualElement, ValueCube> renderPositions,
                                IStyleContext styleContext)
            : base(perspective, surrogateProvider, renderPositions, 
                lastMeasures, styleContext)
        {
            //windowProvider.WindowShown += OnWindowShown;

            
            _imageProvider = imageProvider;
            _testPen = new Pen(Color.Yellow, 1);
            Graphics = nullGraphics;
        }

        //private void OnWindowShown(IVisualHost window)
        //{
        //    //_window = window;
        //    _windowProvider.WindowShown -= OnWindowShown;
        //}

        internal Graphics Graphics { get; set; }

        public override void DrawEllipse<TPoint, TPen>(TPoint center,
                                                       Double radius,
                                                       TPen pen)
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


        public override  void DrawImage<TRectangle>(IImage img, 
                                                    TRectangle destination)
        {
            var dest = GetAbsoluteGdiRectangleF(destination);

            img.UnwrapLocked<Bitmap>(b => Graphics.DrawImage(b, dest));

            //var bmp = img.Unwrap<Bitmap>();
            //Graphics.DrawImage(bmp, dest);
        }

        public override   void DrawImage<TRectangle1, TRectangle2>(IImage img,
                                                                   TRectangle1 sourceRect,
                                                                   TRectangle2 destination)
        {
            var dest = GetAbsoluteGdiRectangleF(destination);
            var src = new Rectangle(Convert.ToInt32(sourceRect.X),
                Convert.ToInt32(sourceRect.Y),
                Convert.ToInt32(sourceRect.Width),
                Convert.ToInt32(sourceRect.Height));

            img.UnwrapLocked<Bitmap>(b => Graphics.DrawImage(b, dest, src, GraphicsUnit.Pixel));

            //var bmp = img.Unwrap<Bitmap>();
            //Graphics.DrawImage(bmp, dest, src, GraphicsUnit.Pixel);
        }

        //public override IImage? GetImage(Stream stream)
        //{
        //    return _imageProvider.GetImage(stream);
        //}

        //public override IImage? GetImage(Stream stream, 
        //                                 Double maximumWidthPct)
        //{
        //    return _imageProvider.GetDeviceScaledImage(stream, maximumWidthPct);

        //}

        //public override IImage GetNullImage()
        //{
        //    return _imageProvider.GetNullImage();
        //    //return _emptyImage ??= new GdiBitmap(new Bitmap(1, 1), null);
        //}

        public override void DrawLine<TPen, TPoint1, TPoint2>(TPen pen,
                                                              TPoint1 pt1,
                                                              TPoint2 pt2)
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

        public override void FillRoundedRectangle<TRectangle, TBrush>(TRectangle rect,
                                                                      TBrush brush,
                                                                      Double cornerRadius)
        {
            if (cornerRadius == 0)
            {
                FillRectangle(rect, brush);
                return;
            }

            var useRect = GetAbsoluteGdiRectangle(rect);
            var useBrush = TypeConverter.GetBrush(brush);

            using (GraphicsPath path = RoundedRect(useRect, cornerRadius))
            {
                Graphics.FillPath(useBrush, path);
            }
        }

        public override void DrawRect<TRectangle, TPen>(TRectangle rect,
                                                        TPen pen)
        {
            var useRect = GetAbsoluteGdiRectangle(rect);
            var usePen = TypeConverter.GetPen(pen);
            Graphics.DrawRectangle(usePen, useRect);
        }

        public override void DrawRoundedRect<TRectangle, TPen>(TRectangle rect,
                                                               TPen pen,
                                                               Double cornerRadius)
        {
            if (cornerRadius == 0)
            {
                DrawRect(rect, pen);
                return;
            }

            var useRect = GetAbsoluteGdiRectangle(rect);
            var usePen = TypeConverter.GetPen(pen);

            using (GraphicsPath path = RoundedRect(useRect, cornerRadius))
            {
                Graphics.DrawPath(usePen, path);
            }
        }

        // https://stackoverflow.com/questions/33853434/how-to-draw-a-rounded-rectangle-in-c-sharp
        private static GraphicsPath RoundedRect(Rectangle bounds, 
                                                Double cornerRadius)
        {
            var radius = Convert.ToInt32(cornerRadius);

            var diameter = radius * 2;
            var size = new System.Drawing.Size(diameter, diameter);
            var arc = new Rectangle(bounds.Location, size);
            
            //don't dispose this here derp
            GraphicsPath path = new GraphicsPath();
            {
                if (radius == 0)
                {
                    path.AddRectangle(bounds);
                    return path;
                }

                // top left arc  
                path.AddArc(arc, 180, 90);

                // top right arc  
                arc.X = bounds.Right - diameter;
                path.AddArc(arc, 270, 90);

                // bottom right arc  
                arc.Y = bounds.Bottom - diameter;
                path.AddArc(arc, 0, 90);

                // bottom left arc 
                arc.X = bounds.Left;
                path.AddArc(arc, 90, 90);

                path.CloseFigure();
                return path;
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

            var color = TypeConverter.GetColor(scb.Color);
            var useFont = TypeConverter.GetFont(font);
            TextRenderer.DrawText(Graphics, s, useFont, loc, color);
        }

        protected override void PushClip<TRectangle>(TRectangle rect)
        {
            _clipCounter++;
            var useRect = TypeConverter.GetRect(rect);
            Graphics.SetClip(useRect);
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

        public override void DrawString<TFont, TBrush, TRectangle>(String s,
                                                                   TFont font,
                                                                   TRectangle location,
                                                                   TBrush brush)
        {
            var rect = GetAbsoluteGdiRectangle(location);
            
            var useBrush = TypeConverter.GetBrush(brush);
            var useFont = TypeConverter.GetFont(font);
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
            var c = GetAbsolutePoint(center);

            var useBrush = TypeConverter.GetBrush(brush);
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

        private RectangleF GetAbsoluteGdiRectangleF<TRectangle>(TRectangle relativeRect)
        where TRectangle : IRectangle
        {
            var absRect = GetAbsoluteRect(relativeRect);
            return TypeConverter.GetRect(absRect);
        }

        private readonly IPen _testPen;

        private Int32 _clipCounter;
        //private IVisualHost? _window;
        //private IImage? _emptyImage;
    }
}