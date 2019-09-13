using System;
using System.Collections.Generic;
using Das.Views.DataBinding;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public class StackPanel<T> : BaseSequentialPanel<T>, ISequentialPanel
    {
        public StackPanel()
        {
        }

        public StackPanel(IDataBinding<T> binding) : base(binding)
        {
        }

        protected override IEnumerable<IVisualElement> GetChildrenToRender() => Children;

        public override IVisualElement DeepCopy()
        {
            var pnl = (StackPanel<T>) base.DeepCopy();
            pnl.Orientation = Orientation;
            return pnl;
        }
    }
}