using System;
using System.Threading.Tasks;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;

namespace Das.Views.Rendering
{
    public interface ISequentialRenderer
    {
        void Arrange(Orientations orientation,
                     IRenderRectangle availableSpace,
                     IRenderContext renderContext);

        ValueSize Measure(IVisualElement container,
                          Orientations orientation,
                          IRenderSize availableSpace,
                          IMeasureContext measureContext);
    }
}