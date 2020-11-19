using System;
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
    }
}