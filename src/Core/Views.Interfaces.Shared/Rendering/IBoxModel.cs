using System;
using Das.Views.Core.Geometry;
using Das.Views.Rendering.Geometry;
using Das.Views.Transforms;

namespace Das.Views.Rendering
{
    public interface IBoxModel
    {
        void Push<TRenderRectangle>(TRenderRectangle rect,
                                    ITransform transform)
            where TRenderRectangle : IRenderRectangle;


        void UpdateCurrent(ValueThickness margin,
                           ValueThickness border);

        void Pop();

        ValueRenderRectangle GetCurrentContentRect();
    }
}
