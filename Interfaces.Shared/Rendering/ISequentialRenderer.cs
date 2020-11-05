using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;

namespace Das.Views.Rendering
{
    public interface ISequentialRenderer
    {
        void Arrange(Orientations orientation,
                     IRenderSize availableSpace, 
                     IRenderContext renderContext);

        Size Measure(IVisualElement container, 
                     IEnumerable<IVisualElement> elements,
                     Orientations orientation, 
                     IRenderSize availableSpace,
                     IMeasureContext measureContext);
    }
}