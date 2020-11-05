using System;
using Das.Views.Rendering;

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

        public new IRenderSize Reduce(Thickness padding)
        {
            return GeometryHelper.Reduce(this, padding);
        }

        public new IRenderSize Minus(ISize subtract)
        {
            return GeometryHelper.Minus(this, subtract);
        }

        public new IRenderSize DeepCopy()
        {
            return new ValueRenderSize(Width, Height, Offset);
        }

        public Boolean Equals(IRenderSize other)
        {
            throw new NotImplementedException();
        }
    }
}
