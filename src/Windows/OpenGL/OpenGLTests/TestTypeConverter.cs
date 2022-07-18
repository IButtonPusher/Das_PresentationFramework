using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Threading;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;
using FontStyle = System.Drawing.FontStyle;
using HatchBrush = Das.Views.Core.Drawing.HatchBrush;
using Pen = System.Drawing.Pen;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using Size = System.Drawing.Size;

namespace OpenGLTests
{
   public static class TestTypeConverter
   {
   static TestTypeConverter()
        {
            //_brushLock = new Object();
            _fonts = new ConcurrentDictionary<IFont, Font>();
            _pens = new ConcurrentDictionary<IPen, Pen>();
            //_brushes = new Dictionary<IBrush, Brush>();
            //_colorBrushes = new Dictionary<IColor, Brush>();

            _threadedBrushes = new ThreadLocal<Dictionary<IBrush, Brush>>(GetNewBrushDictionary);
            _threadedColorBrushes = new ThreadLocal<Dictionary<IColor, Brush>>(GetNewColorBrushDictionary);
        }

        private static Dictionary<IBrush, Brush> GetNewBrushDictionary() => new();

        private static Dictionary<IColor, Brush> GetNewColorBrushDictionary() => new();

        // ReSharper disable once UnusedMember.Global
        public static Boolean Equate(ISize dvSize, Size size)
        {
            return dvSize != null && Convert.ToInt32(dvSize.Width) == size.Width
                                  && Convert.ToInt32(dvSize.Height) == size.Height;
        }

        public static Brush GetBrush(IBrush brush)
        {
            var _brushes = _threadedBrushes.Value;

            //lock (_brushLock)
            {
                //var brushes = Brushes.Value;

                if (_brushes.TryGetValue(brush, out var found))
                    return found;

                switch (brush)
                {
                    case SolidColorBrush scb:
                        found = GetBrush(scb.Color, scb.Opacity);
                        break;

                    case HatchBrush hb:
                        found = new System.Drawing.Drawing2D.HatchBrush(
                            (System.Drawing.Drawing2D.HatchStyle) hb.HatchStyle,
                            GetColor(hb.ForegroundColor),
                            GetColor(hb.BackgroundColor));

                        break;

                    default:
                        throw new NotImplementedException();
                }

                //found = GetBrush(brush.Color);

                _brushes.Add(brush, found);
                return found;
            }
        }

        public static Brush GetBrush(IColor color,
                                     Double opacity)
        {
            //var brushes = ColorBrushes.Value;
            //lock (_brushLock)
            var _colorBrushes = _threadedColorBrushes.Value;
            {
                if (opacity.AreEqualEnough(1.0) && 
                    _colorBrushes.TryGetValue(color, out var found))
                    return found;

                var gcolor = opacity.AreDifferent(1.0)
                    ? GetColor(color, opacity)
                    : GetColor(color);


                found = new SolidBrush(gcolor);
                _colorBrushes.Add(color, found);
                return found;
            }
        }

        public static Color GetColor(IColor color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static Color GetColor(IColor color,
                                     Double opacity)
        {
            return Color.FromArgb(Convert.ToInt32(opacity * 255), color.R, color.G, color.B);
        }

        public static Graphics GetSmoothGraphics(this Image image)
        {
           var g = Graphics.FromImage(image);
           g.SmoothingMode = SmoothingMode.AntiAlias;
           g.InterpolationMode = InterpolationMode.NearestNeighbor;
           g.TextRenderingHint = TextRenderingHint.AntiAlias;
           return g;
        }

        public static Font GetFont(IFont font)
        {
            if (_fonts.TryGetValue(font, out var found))
                return found;
            var style = GetFontStyle(font.FontStyle);
            var family = new FontFamily(font.FamilyName);

            found = new Font(family, (Single) font.Size, style);
            _fonts.TryAdd(font, found);
            return found;
        }

        public static FontStyle GetFontStyle(Das.Views.Core.Writing.FontStyle fontStyle)
        {
            var asInt = (Int32) fontStyle;
            return (FontStyle) asInt;
        }

        public static Pen GetPen(IPen pen)
        {
            if (_pens.TryGetValue(pen, out var found))
                return found;

            var color = GetColor(pen.Color);
            var pen15 = new Pen(color, pen.Thickness);
            _pens.TryAdd(pen, pen15);
            return pen15;
        }

        public static Point GetPoint(IPoint2D point2D)
        {
            return new Point(Convert.ToInt32(point2D.X), Convert.ToInt32(point2D.Y));
        }

        public static Point2D GetPoint(Point point)
        {
            return new Point2D(point.X, point.Y);
        }

        public static Rectangle GetRect(IRectangle rect)
        {
            return new Rectangle(GetPoint(rect.Location), GetSize(rect.Size));
        }

        // ReSharper disable once UnusedMember.Global
        public static RectangleF GetRectF(IRectangle rect)
        {
            return new RectangleF(GetPoint(rect.Location), GetSize(rect.Size));
        }


        public static Size GetSize(ISize size)
        {
            return GetSize(size.Width, size.Height);
        }
        
        public static Size GetSize(Double width,
                                   Double height)
        {
            return new Size(Convert.ToInt32(width), Convert.ToInt32(height));
        }

        private static readonly ConcurrentDictionary<IFont, Font> _fonts;
        private static readonly ConcurrentDictionary<IPen, Pen> _pens;

        //private static readonly Dictionary<IBrush, Brush> _brushes;
        //private static readonly Dictionary<IColor, Brush> _colorBrushes;

        private static readonly ThreadLocal<Dictionary<IBrush, Brush>> _threadedBrushes;
        private static readonly ThreadLocal<Dictionary<IColor, Brush>> _threadedColorBrushes;

        
        //private static readonly Object _brushLock;
    }
}