using System;
using System.Collections.Generic;
using System.Text;
using Das.Views.DataBinding;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public class UniformStackPanel<T> : BaseSequentialPanel<T>
    {
        public UniformStackPanel(IVisualBootstrapper templateResolver) 
            : this(null, templateResolver)
        {
        }

        public UniformStackPanel(IDataBinding<T>? binding,
                                 IVisualBootstrapper visualBootstrapper) 
            : this(binding, visualBootstrapper, new VisualCollection())
            //: base(binding, visualBootstrapper, new SequentialUniformRenderer())
        {
        }

        private UniformStackPanel(IDataBinding<T>? binding,
                                 IVisualBootstrapper visualBootstrapper,
                                 IVisualCollection children) :
            base(binding, visualBootstrapper, children, new SequentialUniformRenderer(children))
        {
        }

        //private SequentialUniformRenderer GetRenderer()
        //{
        //    return new SequentialUniformRenderer(Children);
        //}

       

        //protected override IList<IVisualElement> GetChildrenToRender()
        //{
        //    return Children;
        //}
    }
}
