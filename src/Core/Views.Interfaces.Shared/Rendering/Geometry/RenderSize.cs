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

        public void Reset(Double width,
                          Double height,
                          IPoint2D offset)
        {
           Offset = offset;
           Reset(width, height);
        }

        public IPoint2D Offset { get; set; }


        public ValueRenderRectangle ToFullRectangle()
        {
            return new ValueRenderRectangle(Point2D.Empty, this, Offset);
        }

        public ValueSize ToValueSize()
        {
            return GeometryHelper.ToValueSize(this);
        }


        public Boolean Equals(IRenderSize other)
        {
            throw new NotImplementedException();
        }
    }
}