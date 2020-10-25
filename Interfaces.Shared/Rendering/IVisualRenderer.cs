using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.Views.Rendering
{
    public interface IVisualRenderer
    {
        ISize Measure(ISize availableSpace, 
                      IMeasureContext measureContext);

        void Arrange(ISize availableSpace, 
                     IRenderContext renderContext);
    }
}