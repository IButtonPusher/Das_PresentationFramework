using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Brush = System.Drawing.Brush;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;
using FontStyle = System.Drawing.FontStyle;
using Pen = System.Drawing.Pen;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using Size = System.Drawing.Size;

namespace Das.Gdi.Core
{
    public static class TypeConverter
    {
        static TypeConverter()
        {
            _fonts = new ConcurrentDictionary<IFont, Font>();
            _pens = new ConcurrentDictionary<IPen, Pen>();
        }

        public static Boolean Equate(ISize dvSize, Size size)
        {
            return dvSize != null && Convert.ToInt32(dvSize.Width) == size.Width
                                  && Convert.ToInt32(dvSize.Height) == size.Height;
        }

        public static Brush GetBrush(IBrush brush)
        {
            var brushes = Brushes.Value;

            if (brushes.TryGetValue(brush, out var found))
                return found;

            found = GetBrush(brush.Color);

            brushes.Add(brush, found);
            return found;
        }

        public static Brush GetBrush(IColor color)
        {
            var brushes = ColorBrushes.Value;

            if (brushes.TryGetValue(color, out var found))
                return found;

            var gcolor = GetColor(color);
            found = new SolidBrush(gcolor);
            brushes.Add(color, found);
            return found;
        }

        public static Color GetColor(IColor color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
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

        public static FontStyle GetFontStyle(Views.Core.Writing.FontStyle fontStyle)
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

        public static Point GetPoint(IPoint point)
        {
            return new Point(Convert.ToInt32(point.X), Convert.ToInt32(point.Y));
        }

        public static Views.Core.Geometry.Point GetPoint(Point point)
        {
            return new Views.Core.Geometry.Point(point.X, point.Y);
        }

        public static Rectangle GetRect(IRectangle rect)
        {
            return new Rectangle(GetPoint(rect.Location), GetSize(rect.Size));
        }

        public static RectangleF GetRectF(IRectangle rect)
        {
            return new RectangleF(GetPoint(rect.Location), GetSize(rect.Size));
        }


        public static Size GetSize(ISize size)
        {
            return new Size(Convert.ToInt32(size.Width), Convert.ToInt32(size.Height));
        }

        private static readonly ConcurrentDictionary<IFont, Font> _fonts;
        private static readonly ConcurrentDictionary<IPen, Pen> _pens;

        private static readonly ThreadLocal<Dictionary<IBrush, Brush>> Brushes
            = new ThreadLocal<Dictionary<IBrush, Brush>>(()
                => new Dictionary<IBrush, Brush>());

        private static readonly ThreadLocal<Dictionary<IColor, Brush>> ColorBrushes
            = new ThreadLocal<Dictionary<IColor, Brush>>(()
                => new Dictionary<IColor, Brush>());
    }
}