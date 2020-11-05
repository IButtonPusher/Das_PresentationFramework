using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public class GridPanel<T> : BasePanel<T>
    {
        public override void Arrange(IRenderSize availableSpace, IRenderContext renderContext)
        {
        }

        public override void Dispose()
        {
        }

        public override ISize Measure(IRenderSize availableSpace, 
                                      IMeasureContext measureContext)
        {
            return Size.Empty;
        }
    }
}