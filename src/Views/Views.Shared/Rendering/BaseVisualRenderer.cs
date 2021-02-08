using System;
using Das.Views.Core.Geometry;
using Das.Views.Mvvm;
using Das.Views.Rendering;
using Das.Views.Rendering.Geometry;

namespace Das.Views
{
    public abstract class BaseVisualRenderer : NotifyPropertyChangedBase,
                                               IVisualRenderer
    {
        public abstract void Arrange<TRenderSize>(TRenderSize availableSpace, 
                                                 IRenderContext renderContext)
            where TRenderSize : IRenderSize;

        public abstract ValueSize Measure<TRenderSize>(TRenderSize availableSpace, 
                                                       IMeasureContext measureContext)
            where TRenderSize : IRenderSize;

        public ValueRenderRectangle ArrangedBounds { get; set; }

        public abstract void InvalidateMeasure();

        public abstract void InvalidateArrange();

        public abstract Boolean IsRequiresMeasure { get; }

        public abstract Boolean IsRequiresArrange { get; }

        public abstract void AcceptChanges(ChangeType changeType);
    }
}
