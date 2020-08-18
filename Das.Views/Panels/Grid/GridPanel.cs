using Das.Views.Core.Geometry;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public class GridPanel<T> : BasePanel<T>
    {
        public override ISize Measure(ISize availableSpace, IMeasureContext measureContext) => null!;

        public override void Arrange(ISize availableSpace, IRenderContext renderContext)
        {
        }

        public override void Dispose()
        {
            
        }
    }
}