using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public class GridPanel<T> : BasePanel<T>
    {
        public override void Arrange(IRenderSize availableSpace, 
                                     IRenderContext renderContext)
        {
        }

        public override void Dispose()
        {
        }

        public override ValueSize Measure(IRenderSize availableSpace, 
                                          IMeasureContext measureContext)
        {
            return ValueSize.Empty;
        }

        public GridPanel(IDataBinding<T>? binding, 
                         IVisualBootStrapper templateResolver) : base(binding, templateResolver)
        {
        }

        public GridPanel(IVisualBootStrapper templateResolver) : base(templateResolver)
        {
        }
    }
}