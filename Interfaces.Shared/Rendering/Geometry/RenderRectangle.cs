using System;
using Das.Extensions;
using Das.Views.Core.Geometry;

namespace Das.Views.Rendering.Geometry
{
    public class RenderRectangle : Rectangle, 
                                   IRenderRectangle
    {
        public RenderRectangle(Double x,
                               Double y,
                               Double width,
                               Double height,
                               IPoint2D offset)
            : base(x, y, width, height)
        {
            Offset = offset;
        }

        //public RenderRectangle(Double x,
        //                       Double y,
        //                       Double width,
        //                       Double height) 
        //    : base(x, y, width, height)
        //{
        //    Offset = Point2D.Empty;
        //}

        public RenderRectangle(IRectangle start,
                               Thickness margin,
                               IPoint2D offset) : base(start, margin)
        {
            Offset = offset;
        }

        public RenderRectangle(IPoint2D location,
                               ISize size,
                               IPoint2D offset)
        : base(location, size)
        {
            Offset = offset;
        }

        public Boolean Equals(IRenderSize other)
        {
            return false;
        }

        public RenderRectangle()
        {
            Offset = Point2D.Empty;
        }

        public new RenderRectangle DeepCopy()
        {
            return new RenderRectangle(this, Thickness.Empty, Offset);
        }

        public new IRenderSize Minus(ISize subtract)
        {
            return GeometryHelper.Minus(this, subtract);
        }

        //new IRenderSize IRenderRectangle.Size => new ValueRenderSize(base.Size);

        public IPoint2D Offset { get; set; }

        IRenderSize IRenderSize.Reduce(Thickness padding)
        {
            return GeometryHelper.Reduce(this, padding);
        }

        IRenderSize IRenderSize.DeepCopy()
        {
            return new ValueRenderSize(Width, Height, Offset);
        }

        public static RenderRectangle operator +(RenderRectangle rect, Thickness margin)
        {
            if (margin == null)
                return rect.DeepCopy();

            return new RenderRectangle(rect.X, rect.Y,
                rect.Width - (margin.Left + margin.Right),
                rect.Height - (margin.Top + margin.Bottom), rect.Offset);
        }

        public static RenderRectangle operator +(RenderRectangle rect, Point2D location)
        {
            if (location == null)
                return rect.DeepCopy();

            return new RenderRectangle(rect.X + location.X, rect.Y + location.Y,
                rect.Width, rect.Height, rect.Offset);
        }

        public static RenderRectangle operator +(RenderRectangle rect, Size size)
        {
            if (size == null)
                return rect.DeepCopy();

            return new RenderRectangle(rect.X + size.Width, rect.Y + size.Height,
                rect.Width, rect.Height, rect.Offset);
        }

        public static RenderRectangle? operator *(RenderRectangle? rect, Double val)
        {
            if (val.AreEqualEnough(1))
                return rect;

            if (rect == null)
                return null;

            return new RenderRectangle(rect.X, rect.Y,
                rect.Size.Width * val, rect.Size.Height * val,
                rect.Offset);
        }

        IRenderSize IRenderRectangle.Size => new ValueRenderSize(Width, Height, Offset);
    }
}
