using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;
using Das.Gdi.Core;
using Das.Views;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;
using Das.Views.Styles;
using Gdi.Shared;
using Color = Das.Views.Core.Drawing.Color;
using Pen = Das.Views.Core.Drawing.Pen;
using Rectangle = System.Drawing.Rectangle;
using Size = System.Drawing.Size;

namespace Das.Gdi
{
    public class GdiRenderContext : BaseRenderContext
    {
        public GdiRenderContext(IViewPerspective perspective,
                                Graphics nullGraphics,
                                IVisualSurrogateProvider surrogateProvider,
                                Dictionary<IVisualElement, ValueSize> lastMeasures,
                                Dictionary<IVisualElement, ValueCube> renderPositions,
                                IStyleContext styleContext,
                                IVisualLineage visualLineage)
            : base(perspective, surrogateProvider, renderPositions,
                lastMeasures, styleContext, visualLineage)
        {
            _testPen = new Pen(Color.Yellow, 1);
            Graphics = nullGraphics;
        }

        public Graphics Graphics { get; set; }

        public override void DrawEllipse<TPoint, TPen>(TPoint center,
                                                       Double radius,
                                                       TPen pen)
        {
            var c = GetAbsolutePoint(center);

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

        //protected IGraphicsPath GetGraphicsPath()
        //{
        //    return new GdiGraphicsPath();
        //}

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
            //if (cornerRadius == 0)
            if (cornerRadii.IsEmpty)
            {
                DrawRect(rect, pen);
                return;
            }

            //var useRect = GetAbsoluteGdiRectangle(rect);
            var useRect = GetAbsoluteRect(rect);
            var usePen = GdiTypeConverter.GetPen(pen);

            using (var path = new GdiGraphicsPath())
            {
                CreateRoundedRectangle(path, useRect, cornerRadii);
                Graphics.DrawPath(usePen, path.Path);
            }

            //using (GraphicsPath path = CreateRoundedRectangle(useRect, cornerRadii))
            //{
            //    Graphics.DrawPath(usePen, path);
            //}
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

            var color = GdiTypeConverter.GetColor(scb.Color);
            var useFont = GdiTypeConverter.GetFont(font);
            TextRenderer.DrawText(Graphics, s, useFont, loc, color);
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
            var c = GetAbsolutePoint(center);

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
            Graphics.FillRectangle(useBrush, useRect);
        }

        public override void FillRoundedRectangle<TRectangle, TBrush, TThickness>(TRectangle rect,
                                                                      TBrush brush,
                                                                      TThickness cornerRadii)
        {
            //if (cornerRadius == 0)
            if (cornerRadii.IsEmpty)
            {
                FillRectangle(rect, brush);
                return;
            }

            //var useRect = GetAbsoluteGdiRectangle(rect);
            var useRect = GetAbsoluteRect(rect);
            var useBrush = GdiTypeConverter.GetBrush(brush);

            using (var path = new GdiGraphicsPath())
            {
                CreateRoundedRectangle(path, useRect, cornerRadii);
                Graphics.FillPath(useBrush, path.Path);
            }

            //using (GraphicsPath path = CreateRoundedRectangle(useRect, cornerRadii))
            //{
            //    Graphics.FillPath(useBrush, path);
            //}
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
            var to = GetAbsolutePoint(relativePoint2D);
            return new Point(Convert.ToInt32(to.X),
                Convert.ToInt32(to.Y));
        }

        private Rectangle GetAbsoluteGdiRectangle(
            IRectangle relativeRect)
        {
            var absRect = GetAbsoluteRect(relativeRect);
            return GdiTypeConverter.GetRect(absRect);
        }

        private RectangleF GetAbsoluteGdiRectangleF<TRectangle>(TRectangle relativeRect)
            where TRectangle : IRectangle
        {
            var absRect = GetAbsoluteRect(relativeRect);
            return GdiTypeConverter.GetRect(absRect);
        }

        //// https://github.com/koszeggy/KGySoft.Drawing/blob/625e71e38a0d2e2c94821629f34e797ceae4015c/KGySoft.Drawing/Drawing/_Extensions/GraphicsExtensions.cs#L287
        //private static GraphicsPath CreateRoundedRectangle<TThickness>(Rectangle bounds, 
        //                                                               TThickness cornerRadii)
        //where TThickness : IThickness
        //{
        //    var radiusTopLeft = Convert.ToInt32(cornerRadii.Left);
        //    var radiusTopRight = Convert.ToInt32(cornerRadii.Top);
        //    var radiusBottomRight = Convert.ToInt32(cornerRadii.Right);
        //    var radiusBottomLeft = Convert.ToInt32(cornerRadii.Bottom);

        //    var size = new Size(radiusTopLeft << 1, radiusTopLeft << 1);
        //    var arc = new Rectangle(bounds.Location, size);
        //    GraphicsPath path = new GraphicsPath();

        //    // top left arc
        //    if (radiusTopLeft == 0)
        //        path.AddLine(arc.Location, arc.Location);
        //    else
        //        path.AddArc(arc, 180, 90);

        //    // top right arc
        //    if (radiusTopRight != radiusTopLeft)
        //    {
        //        size = new Size(radiusTopRight << 1, radiusTopRight << 1);
        //        arc.Size = size;
        //    }

        //    arc.X = bounds.Right - size.Width;
        //    if (radiusTopRight == 0)
        //        path.AddLine(arc.Location, arc.Location);
        //    else
        //        path.AddArc(arc, 270, 90);

        //    // bottom right arc
        //    if (radiusTopRight != radiusBottomRight)
        //    {
        //        size = new Size(radiusBottomRight << 1, radiusBottomRight << 1);
        //        arc.X = bounds.Right - size.Width;
        //        arc.Size = size;
        //    }

        //    arc.Y = bounds.Bottom - size.Height;
        //    if (radiusBottomRight == 0)
        //        path.AddLine(arc.Location, arc.Location);
        //    else
        //        path.AddArc(arc, 0, 90);

        //    // bottom left arc
        //    if (radiusBottomRight != radiusBottomLeft)
        //    {
        //        arc.Size = new Size(radiusBottomLeft << 1, radiusBottomLeft << 1);
        //        arc.Y = bounds.Bottom - arc.Height;
        //    }

        //    arc.X = bounds.Left;
        //    if (radiusBottomLeft == 0)
        //        path.AddLine(arc.Location, arc.Location);
        //    else
        //        path.AddArc(arc, 90, 90);

        //    path.CloseFigure();
        //    return path;
        //}
        

        //// https://stackoverflow.com/questions/33853434/how-to-draw-a-rounded-rectangle-in-c-sharp
        
        //private static GraphicsPath RoundedRect<TThickness>(Rectangle bounds,
        //                                                    TThickness cornerRadii)
        //where TThickness : IThickness
        //{
        //    //var radius = Convert.ToInt32(cornerRadius);

        //    var tlRadius = Convert.ToInt32(cornerRadii.Left);
        //    var tlDiameter = tlRadius * 2;
        //    var tlSize = new Size(tlDiameter, tlDiameter);
        //    var tlArc = new Rectangle(bounds.Location, tlSize);

        //    //var arc = new Rectangle(bounds.Left, bounds.Top,)

        //    var trRadius = Convert.ToInt32(cornerRadii.Top);
        //    var trDiameter = trRadius * 2;
        //    var trSize = new Size(trDiameter, trDiameter);
        //    var trArc = new Rectangle(bounds.Location, trSize);

        //    var brRadius = Convert.ToInt32(cornerRadii.Right);
        //    var brDiameter = brRadius * 2;
        //    var brSize = new Size(brDiameter, brDiameter);
        //    var brArc = new Rectangle(bounds.Location, brSize);

        //    var blRadius = Convert.ToInt32(cornerRadii.Bottom);
        //    var blDiameter = blRadius * 2;
        //    var blSize = new Size(blDiameter, blDiameter);
        //    var blArc = new Rectangle(bounds.Location, blSize);

        //    var arc = new Rectangle(bounds.Location.X, bounds.Location.Y,
        //        tlRadius + trRadius, brRadius + blRadius);

        //    //var diameter = radius * 2;
        //    //var size = new Size(diameter, diameter);
        //    //var arc = new Rectangle(bounds.Location, size);

        //    //don't dispose this here derp
        //    GraphicsPath path = new GraphicsPath();
        //    {
        //        //if (radius == 0)
        //        if (cornerRadii.IsEmpty)
        //        {
        //            path.AddRectangle(bounds);
        //            return path;
        //        }

        //        // top left arc  
        //        path.AddArc(tlArc, 180, 90);

        //        // top right arc  
        //        tlArc.X = bounds.Right - diameter;
        //        path.AddArc(tlArc, 270, 90);

        //        // bottom right arc  
        //        brArc.Y = bounds.Bottom - diameter;
        //        path.AddArc(brArc, 0, 90);

        //        // bottom left arc 
        //        blArc.X = bounds.Left;
        //        path.AddArc(blArc, 90, 90);

        //        path.CloseFigure();
        //        return path;
        //    }
        //}

        private readonly IPen _testPen;
        private Int32 _clipCounter;
    }
}