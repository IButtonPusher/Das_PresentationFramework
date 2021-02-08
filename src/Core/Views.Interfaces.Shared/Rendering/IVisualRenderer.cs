using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.Rendering.Geometry;

namespace Das.Views.Rendering
{
    /// <summary>
    /// Base interface for a type that can be measured and drawn onto a portion of the screen
    /// </summary>
    public interface IVisualRenderer : IMeasureAndArrange
    {
        void Arrange<TRenderSize>(TRenderSize availableSpace,
                                  IRenderContext renderContext)
            where TRenderSize : IRenderSize;

        ValueSize Measure<TRenderSize>(TRenderSize availableSpace,
                                       IMeasureContext measureContext)
            where TRenderSize : IRenderSize;

        ValueRenderRectangle ArrangedBounds { get; set; }
    }
}