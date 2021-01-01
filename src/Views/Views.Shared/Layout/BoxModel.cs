using System;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;
using Das.Views.Rendering.Geometry;
using Das.Views.Transforms;

namespace Das.Views.Layout
{
    public class BoxModel : IBoxModel
    {
        public void Push<TRenderRectangle>(TRenderRectangle rect, 
                                           ITransform transform) 
            where TRenderRectangle : IRenderRectangle
        {
            throw new NotImplementedException();
        }

        public void UpdateCurrent(ValueThickness margin, 
                                  ValueThickness border)
        {
            throw new NotImplementedException();
        }

        public void Pop()
        {
            throw new NotImplementedException();
        }

        public ValueRenderRectangle GetCurrentContentRect()
        {
            throw new NotImplementedException();
        }
    }
}
