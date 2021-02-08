using System;
using System.Threading.Tasks;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;

namespace Das.Views.Rendering
{
    public interface ISequentialRenderer
    {
        void Arrange<TRenderRect>(Orientations orientation,
                                  TRenderRect availableSpace,
                                  IRenderContext renderContext)
            where TRenderRect : IRenderRectangle;

        ValueSize Measure<TRenderSize>(IVisualElement container,
                                      Orientations orientation,
                                      TRenderSize availableSpace,
                                      IMeasureContext measureContext)
            where TRenderSize : IRenderSize;
    }
}