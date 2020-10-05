using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.Views.Rendering
{
    public interface IVisualRenderer
    {
        void Arrange(ISize availableSpace, 
                     IRenderContext renderContext);

        ISize Measure(ISize availableSpace, 
                      IMeasureContext measureContext);
    }
}