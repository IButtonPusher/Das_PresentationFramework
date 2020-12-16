using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
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
            _brushLock = new Object();
            _fonts = new ConcurrentDictionary<IFont, Font>();
            _pens = new ConcurrentDictionary<IPen, Pen>();
            _brushes = new Dictionary<IBrush, Brush>();
            _colorBrushes = new Dictionary<IColor, Brush>();
        }

        public static Boolean Equate(ISize dvSize, Size size)
        {
            return dvSize != null && Convert.ToInt32(dvSize.Width) == size.Width
                                  && Convert.ToInt32(dvSize.Height) == size.Height;
        }

        public static Brush GetBrush(IBrush brush)
        {
            lock (_brushLock)
            {
                //var brushes = Brushes.Value;

                if (_brushes.TryGetValue(brush, out var found))
                    return found;

                switch (brush)
                {
                    case SolidColorBrush scb:
                        found = GetBrush(scb.Color);
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

        public static Brush GetBrush(IColor color)
        {
            //var brushes = ColorBrushes.Value;
            lock (_brushLock)
            {
                if (_colorBrushes.TryGetValue(color, out var found))
                    return found;

                var gcolor = GetColor(color);
                found = new SolidBrush(gcolor);
                _colorBrushes.Add(color, found);
                return found;
            }
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

        private static readonly Dictionary<IBrush, Brush> _brushes;
        private static readonly Dictionary<IColor, Brush> _colorBrushes;
        private static readonly Object _brushLock;

        //private static readonly ThreadLocal<Dictionary<IBrush, Brush>> Brushes
        //    = new ThreadLocal<Dictionary<IBrush, Brush>>(()
        //        => new Dictionary<IBrush, Brush>());

        //private static readonly ThreadLocal<Dictionary<IColor, Brush>> ColorBrushes
        //    = new ThreadLocal<Dictionary<IColor, Brush>>(()
        //        => new Dictionary<IColor, Brush>());
    }
}