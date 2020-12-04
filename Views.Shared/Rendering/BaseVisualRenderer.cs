using System;
using Das.Views.Core.Geometry;
using Das.Views.Mvvm;
using Das.Views.Rendering;

namespace Das.Views
{
    public abstract class BaseVisualRenderer : NotifyPropertyChangedBase,
                                               IVisualRenderer
    {
        public abstract void Arrange(IRenderSize availableSpace, 
                                     IRenderContext renderContext);

        public abstract ValueSize Measure(IRenderSize availableSpace, 
                                          IMeasureContext measureContext);

        public abstract void InvalidateMeasure();

        public abstract void InvalidateArrange();

        public abstract Boolean IsRequiresMeasure { get; }

        public abstract Boolean IsRequiresArrange { get; }

        public abstract void AcceptChanges(ChangeType changeType);
    }
}
