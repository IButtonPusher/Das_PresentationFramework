using System;
using System.Threading.Tasks;

namespace Das.Views.Panels
{
    public class StackPanel : BaseSequentialPanel
    {
        public StackPanel(IVisualBootstrapper visualBootstrapper)
        : base(visualBootstrapper)
        {
        }

        //public StackPanel(IDataBinding<T> binding,
        //                  IVisualBootstrapper visualBootstrapper) 
        //    : base(binding, visualBootstrapper)
        //{
        //}

        //public override IVisualElement DeepCopy()
        //{
        //    var pnl = (StackPanel<T>) base.DeepCopy();
        //    pnl.Orientation = Orientation;
        //    return pnl;
        //}

      
        public override void Dispose()
        {
            base.Dispose();
            _children.Dispose();

            //foreach (var child in Children)
            //    child.Dispose();
        }

        //protected override IList<IVisualElement> GetChildrenToRender()
        //{
        //    return Children;
        //}
    }
}