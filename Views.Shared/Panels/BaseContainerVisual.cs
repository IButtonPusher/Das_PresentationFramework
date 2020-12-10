using System;
using System.ComponentModel;
using Das.Views.DataBinding;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public abstract class BaseContainerVisual : BindableElement,
                                              IContainerVisual
    {
        public BaseContainerVisual(IVisualBootstrapper visualBootstrapper) 
            : base(visualBootstrapper)
        {
        }

        public virtual Boolean IsChanged
        {
            get => IsRequiresMeasure || IsRequiresArrange;
        }

        public virtual void AcceptChanges()
        {
            AcceptChanges(ChangeType.Measure);
            AcceptChanges(ChangeType.Arrange);
        }

    }
}
