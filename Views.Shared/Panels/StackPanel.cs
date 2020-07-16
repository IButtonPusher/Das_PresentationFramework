using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.DataBinding;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public class StackPanel<T> : BaseSequentialPanel<T>
    {
        public StackPanel()
        {
        }

        public StackPanel(IDataBinding<T> binding) : base(binding)
        {
        }

        public override IVisualElement DeepCopy()
        {
            var pnl = (StackPanel<T>) base.DeepCopy();
            pnl.Orientation = Orientation;
            return pnl;
        }

        public override void Dispose()
        {
            for (var c = 0; c < Children.Count; c++)
            {
                Children[c].Dispose();
            }
        }

        protected override IEnumerable<IVisualElement> GetChildrenToRender()
        {
            return Children;
        }
    }
}