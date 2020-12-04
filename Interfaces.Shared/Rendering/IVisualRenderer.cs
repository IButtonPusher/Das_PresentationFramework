using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.Views.Rendering
{
    public interface IVisualRenderer : IMeasureAndArrange
    {
        void Arrange(IRenderSize availableSpace,
                     IRenderContext renderContext);

        ValueSize Measure(IRenderSize availableSpace,
                          IMeasureContext measureContext);

       
    }
}