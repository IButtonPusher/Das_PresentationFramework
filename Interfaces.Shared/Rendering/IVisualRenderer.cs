using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.Views.Rendering
{
    public interface IVisualRenderer
    {
        void Arrange(IRenderSize availableSpace,
                     IRenderContext renderContext);

        ValueSize Measure(IRenderSize availableSpace,
                          IMeasureContext measureContext);

        void InvalidateMeasure();

        void InvalidateArrange();

        Boolean IsRequiresMeasure { get; }

        Boolean IsRequiresArrange { get; }
    }
}