using System;
using System.Threading.Tasks;
using Das.Views.Rendering;
using Das.Views.Rendering.Geometry;

namespace Das.Views.Core.Geometry
{
    public class RenderSize : Size,
                              IRenderSize
    {
        public RenderSize(Double width,
                          Double height,
                          IPoint2D offset)
            : base(width, height)
        {
            Offset = offset;
        }

        public RenderSize(Double width,
                          Double height)
            : this(width, height, Point2D.Empty)
        {
        }

        public RenderSize()
        {
            Offset = Point2D.Empty;
        }

        public IPoint2D Offset { get; set; }

        //public new IRenderSize Reduce(Thickness padding)
        //{
        //    return GeometryHelper.Reduce(this, padding);
        //}

        //public IRenderSize MinusVertical(ISize subtract)
        //{
        //    return GeometryHelper.MinusVertical(this, subtract);
        //}

        //public new IRenderSize PlusVertical(ISize adding)
        //{
        //    return GeometryHelper.PlusRenderVertical(this, adding);
        //}

        public ValueRenderRectangle ToFullRectangle()
        {
            return new ValueRenderRectangle(Point2D.Empty, this, Offset);
        }

        public ValueSize ToValueSize()
        {
            return GeometryHelper.ToValueSize(this);
        }


        //public new IRenderSize Minus(ISize subtract)
        //{
        //    return GeometryHelper.Minus(this, subtract);
        //}

        //public new IRenderSize DeepCopy()
        //{
        //    return new ValueRenderSize(Width, Height, Offset);
        //}

        public Boolean Equals(IRenderSize other)
        {
            throw new NotImplementedException();
        }
    }
}