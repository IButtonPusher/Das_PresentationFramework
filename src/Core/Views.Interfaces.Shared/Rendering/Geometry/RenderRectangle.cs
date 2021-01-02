using System;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Views.Core.Geometry;

namespace Das.Views.Rendering.Geometry
{
    public class RenderRectangle : Rectangle,
                                   IRenderRectangle
    {
        private IPoint2D _offset;

        public RenderRectangle(Double x,
                               Double y,
                               Double width,
                               Double height,
                               IPoint2D offset)
            : base(x, y, width, height)
        {
            _offset = offset;
        }

        public RenderRectangle(IRectangle start,
                               Thickness margin,
                               IPoint2D offset) : base(start, margin) 
        {
            _offset = offset;
        }

        public RenderRectangle(IPoint2D location,
                               ISize size,
                               IPoint2D offset)
            : base(location, size)
        {
            _offset = offset;
        }

        public RenderRectangle()
        {
            _offset = Point2D.Empty;
        }

        public void Update<TPoint, TRenderRectangle, TThickness>(TRenderRectangle rect,
                                                                 TPoint parentOffset,
                                                                 TThickness margin,
                                                                 TThickness border)
            where TPoint : IPoint2D
            where TRenderRectangle : IRenderRectangle
            where TThickness : IThickness
        {
            _left = rect.Left + margin.Left - parentOffset.X;
            _top = rect.Top + margin.Top - parentOffset.Y;
            _w = rect.Width - margin.Width;
            _h = rect.Height - margin.Height;
            _offset = rect.Size.Offset;


            if (border.IsEmpty)
                return;

            _left += border.Left;
            _top += border.Top;
            _w -= border.Width;
            _h -= border.Height;
        }

        public void Update<TPoint>(Double x,
                                   Double y,
                                   Double width,
                                   Double height,
                                   TPoint parentOffset,
                                   TPoint offset,
                                   Thickness margin,
                                   Thickness border)
            where TPoint : IPoint2D
        {
            _left = x + margin.Left - parentOffset.X;
           _top = y + margin.Top - parentOffset.Y;
           _w = width - margin.Width;
           _h = height - margin.Height;
           _offset = offset;


           if (border.IsEmpty) 
               return;

           _left += border.Left;
           _top += border.Top;
           _w -= border.Width;
           _h -= border.Height;
        }


        public Boolean Equals(IRenderSize other)
        {
            return false;
        }

        //public new IRenderSize Minus(ISize subtract)
        //{
        //    return GeometryHelper.Minus(this, subtract);
        //}

        //IRenderSize IRenderSize.PlusVertical(ISize adding)
        //{
        //    return GeometryHelper.PlusRenderVertical(this, adding);
        //}

        //public IRenderSize MinusVertical(ISize subtract)
        //{
        //    return GeometryHelper.MinusVertical(this, subtract);
        //}

        public IPoint2D Offset  
        {
            get => _offset;
        }

        //IRenderSize IRenderSize.Reduce(Thickness padding)
        //{
        //    return GeometryHelper.Reduce(this, padding);
        //}

        //public ValueRenderRectangle ToFullRectangle()
        //{
        //    return new ValueRenderRectangle(0,0, Width, Height, Offset);
        //}

        //public ValueSize ToValueSize()
        //{
        //    return GeometryHelper.ToValueSize(this);
        //}

        //IRenderSize IRenderSize.DeepCopy()
        //{
        //    return new ValueRenderSize(Width, Height, Offset);
        //}

        IRenderSize IRenderRectangle.Size => new ValueRenderSize(Width, Height, Offset);

        TRectangle IRenderRectangle.Reduce<TRectangle>(Double left, 
                                             Double top, 
                                             Double right, 
                                             Double bottom)
        {
            var res = new RenderRectangle(X + left, Top + top,
                Width - (left + right),
                Height - (top + bottom), Offset);

            if (res is TRectangle fku)
                return fku;
            throw new InvalidOperationException();
        }

        public new RenderRectangle DeepCopy()
        {
            return new RenderRectangle(this, Thickness.Empty, Offset);
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

        public static RenderRectangle operator *(RenderRectangle rect, 
                                                 Double val)
        {
            if (val.AreEqualEnough(1))
                return rect;

            if (rect == null)
                return null!;

            return new RenderRectangle(rect.X * val,
                rect.Y * val,
                rect.Size.Width * val,
                rect.Size.Height * val,
                new ValuePoint2D(rect.Offset.X * val,
                    rect.Offset.Y * val));
        }

        public Boolean Equals(IRenderRectangle other)
        {
            return GeometryHelper.AreRenderRectsEquals(this, other);
        }
    }
}