using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.Views.Rendering
{
    public interface IVisualRenderer
    {
        ISize Measure(IRenderSize availableSpace, 
                      IMeasureContext measureContext);

        void Arrange(IRenderSize availableSpace, 
                     IRenderContext renderContext);
    }
}