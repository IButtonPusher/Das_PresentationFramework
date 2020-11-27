﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.DataBinding;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public class StackPanel<T> : BaseSequentialPanel<T>
    {
        public StackPanel(IVisualBootStrapper templateResolver)
        : base(templateResolver)
        {
        }

        public StackPanel(IDataBinding<T> binding,
                          IVisualBootStrapper visualBootStrapper) 
            : base(binding, visualBootStrapper)
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
            foreach (var child in Children)
                child.Dispose();
        }

        protected override IList<IVisualElement> GetChildrenToRender()
        {
            return Children;
        }
    }
}