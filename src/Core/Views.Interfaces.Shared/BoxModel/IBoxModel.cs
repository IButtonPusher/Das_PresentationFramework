using System;
using Das.Views.Core.Geometry;
using Das.Views.Rendering.Geometry;
using Das.Views.Transforms;

namespace Das.Views.Rendering
{
    public interface IBoxModel
    {

        ValueRenderRectangle ComputeContentBounds<TRenderRectangle, TThickness>(TRenderRectangle rect,
                                                                             TThickness margin,
                                                                             TThickness border)
            where TRenderRectangle : IRenderRectangle
            where TThickness : IThickness;


        void PushTransform(TransformationMatrix transform);
        
        ValueRenderRectangle PushContentBounds(ValueRenderRectangle bounds);


        void PopTransform();

        void PopCurrentBox();

        ValuePoint2D GetAbsolutePoint<TPoint>(TPoint relativePoint2D,
                                              Double zoomLevel)
            where TPoint : IPoint2D;

        ValueRectangle GetAbsoluteRect<TRectangle>(TRectangle relativeRect,
                                                   Double zoomLevel)
            where TRectangle : IRectangle;
    }
}
