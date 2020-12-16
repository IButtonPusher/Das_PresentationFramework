using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.Views.Rendering
{
    /// <summary>
    /// Base interface for a type that can be measured and drawn onto a portion of the screen
    /// </summary>
    public interface IVisualRenderer : IMeasureAndArrange
    {
        void Arrange(IRenderSize availableSpace,
                     IRenderContext renderContext);

        ValueSize Measure(IRenderSize availableSpace,
                          IMeasureContext measureContext);
    }
}