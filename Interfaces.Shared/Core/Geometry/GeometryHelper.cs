using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Views.Rendering;

namespace Das.Views.Core.Geometry
{
    public static class GeometryHelper
    {
        public static Boolean AreSizesEqual<TSize>(TSize left,
                                                   ISize? other)
            where TSize : ISize
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(left, other)) return true;

            return left.Width.AreEqualEnough(other.Width) &&
                   left.Height.AreEqualEnough(other.Height);
        }

        public static Boolean IsRectangleContains<TRect>(TRect rectangle,
                                                         IPoint2D point)
            where TRect : IRectangle
        {
            return point.X >= rectangle.Left
                   && point.X <= rectangle.Right
                   && point.Y >= rectangle.Top
                   && point.Y <= rectangle.Bottom;
        }

        public static Boolean IsRectangleContains<TRect>(TRect rectangle,
                                                         Int32 x,
                                                         Int32 y)
            where TRect : IRectangle
        {
            return x >= rectangle.Left
                   && x <= rectangle.Right
                   && y >= rectangle.Top
                   && y <= rectangle.Bottom;
        }

        public static Boolean IsRectangleContains<TRect>(TRect rectangle,
                                                         Double x,
                                                         Double y)
            where TRect : IRectangle
        {
            return x >= rectangle.Left
                   && x <= rectangle.Right
                   && y >= rectangle.Top
                   && y <= rectangle.Bottom;
        }

        public static ISize Minus(ISize original,
                                  ISize takeAway)
        {
            return new ValueSize(original.Width - takeAway.Width,
                original.Height - takeAway.Height);
        }

        public static ISize PlusVertical(ISize original,
                                         ISize takeAway)
        {
            return new ValueSize(Math.Max(original.Width, takeAway.Width),
                original.Height + takeAway.Height);
        }

        public static ValueSize PlusVertical(ValueSize original,
                                             ISize takeAway)
        {
            return new ValueSize(Math.Max(original.Width, takeAway.Width),
                original.Height + takeAway.Height);
        }

        public static IRenderSize PlusRenderVertical(IRenderSize original,
                                               ISize takeAway)
        {
            return new ValueRenderSize(Math.Max(original.Width, takeAway.Width),
                original.Height + takeAway.Height, original.Offset);
        }

        public static IRenderSize MinusVertical(IRenderSize original,
                                               ISize takeAway)
        {
            return new ValueRenderSize(Math.Max(original.Width, takeAway.Width),
                original.Height - takeAway.Height, original.Offset);
        }

        public static IRenderSize Minus(IRenderSize original,
                                        ISize takeAway)
        {
            return new ValueRenderSize(original.Width - takeAway.Width,
                original.Height - takeAway.Height, original.Offset);
        }

        public static ISize Reduce(ISize size,
                                   Thickness margin)
        {
            if (margin == null)
                return size.DeepCopy();

            return new ValueSize(size.Width - (margin.Left + margin.Right),
                size.Height - (margin.Top + margin.Bottom));
        }

        public static IRenderSize Reduce(IRenderSize size,
                                         Thickness margin)
        {
            if (margin == null)
                return size.DeepCopy();

            return new ValueRenderSize(size.Width - (margin.Left + margin.Right),
                size.Height - (margin.Top + margin.Bottom), size.Offset);
        }

        public static ValueSize ToValueSize(IRenderSize rect)
        {
            return new ValueSize(rect.Width, rect.Height);
        }

        public static ValueIntRectangle GetUnion<TRoundedRectangle>(TRoundedRectangle a,
                                                                    IRoundedRectangle b)
            where TRoundedRectangle : IRoundedRectangle
        {

            var x1 = Math.Min(a.X, b.X);
            var x2 = Math.Max(a.X + a.Width, b.X + b.Width);
            var y1 = Math.Min(a.Y, b.Y);
            var y2 = Math.Max(a.Y + a.Height, b.Y + b.Height);
            return new ValueIntRectangle(x1, y1, x2 - x1, y2 - y1);
        }

        public static ValueIntRectangle GetUnion<TRoundedRectangle>(TRoundedRectangle me,
                                                                    IEnumerable<IRoundedRectangle> others)
        where TRoundedRectangle : IRoundedRectangle
        {
            //var me = this as IRoundedRectangle;

            var x1 = me.X;
            var x2 = me.X + me.Width;
            var y1 = me.Y;
            var y2 = me.Y + me.Height;

            foreach (var b in others)
            {
                x1 = Math.Min(x1, b.X);
                x2 = Math.Max(x2, b.X + b.Width);
                y1 = Math.Min(y1, b.Y);
                y2 = Math.Max(y2, b.Y + b.Height);
            }

            return new ValueIntRectangle(x1, y1, x2 - x1, y2 - y1);
        }

        public static Double CenterX(ISize outer,
                                     ISize inner)
        {
            if (inner.Width > outer.Width)
                return 0;

            return (outer.Width - inner.Width) / 2;
        }

        public static Double CenterY(ISize outer,
                                     ISize inner)
        {
            if (inner.Height > outer.Height)
                return 0;

            return (outer.Height - inner.Height) / 2;
        }

        public static IPoint2D Offset(IPoint2D point,
                                      Double pct)
        {
            return new ValuePoint2D(point.X - (point.X * pct), 
                point.Y - (point.Y * pct));
        }

        public static ISize Divide(ISize size,
                                   Double pct)
        {
            return new ValueSize(size.Width - (size.Width * pct), 
                size.Height - (size.Height * pct));
        }
    }
}