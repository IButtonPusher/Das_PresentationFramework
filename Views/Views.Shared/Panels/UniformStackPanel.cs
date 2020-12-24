using System;
using Das.Views.Collections;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public class UniformStackPanel : BaseSequentialPanel
    {
        //public UniformStackPanel(IVisualBootstrapper templateResolver) 
        //    : this(null, templateResolver)
        //{
        //}

        public UniformStackPanel(//IDataBinding<T>? binding,
                                 IVisualBootstrapper visualBootstrapper) 
            //: this(binding, visualBootstrapper, new VisualCollection())
                : this(visualBootstrapper, new VisualCollection())
            
        {
        }

        private UniformStackPanel(//IDataBinding<T>? binding,
                                 IVisualBootstrapper visualBootstrapper,
                                 IVisualCollection children)
            //: base(binding, visualBootstrapper, children, new SequentialUniformRenderer(children))
                : base(visualBootstrapper, children, new SequentialUniformRenderer(children))
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
