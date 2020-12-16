using System;
using System.Threading.Tasks;
using Das.Views.Collections;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public class WrapPanel : BaseSequentialPanel
    {
        public WrapPanel(IVisualBootstrapper visualBootstrapper) 
            : this(visualBootstrapper, new VisualCollection())
            //: base(default!, new SequentialRenderer(GetChildCollection(), true))
        {
        }

        private WrapPanel(IVisualBootstrapper visualBootstrapper,
                          IVisualCollection children) :
            base(visualBootstrapper, children, new SequentialUniformRenderer(children))
        {
        }


        //public override void Dispose()
        //{
        //}

        //protected override IList<IVisualElement> GetChildrenToRender()
        //{
        //    return Children;
        //}
        protected override void OnDistributeDataContextToChildren(Object? newValue)
        {
            throw new NotImplementedException();
        }
    }
}