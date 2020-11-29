using System;
using System.Collections.Generic;
using System.Text;
using Das.Views.DataBinding;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public class UniformStackPanel<T> : BaseSequentialPanel<T>
    {
        public UniformStackPanel(IDataBinding<T>? binding,
                                 IVisualBootstrapper visualBootstrapper) :
            base(binding, visualBootstrapper, new SequentialUniformRenderer())
        {
        }

        public UniformStackPanel(IVisualBootstrapper templateResolver) 
            : base(templateResolver, new SequentialUniformRenderer())
        {
        }

        protected override IList<IVisualElement> GetChildrenToRender()
        {
            return Children;
        }
    }
}
