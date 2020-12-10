using System;
using System.ComponentModel;
using Das.Views.DataBinding;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public abstract class BaseContainerVisual<T> : BindableElement<T>,
                                                IContainerVisual
    {
        public BaseContainerVisual(IVisualBootstrapper visualBootstrapper) 
            : base(visualBootstrapper)
        {
        }

        protected BaseContainerVisual(IVisualBootstrapper visualBootstrapper,
                                      IDataBinding<T>? binding)
         : base(binding, visualBootstrapper )
        {

        }
        
        public virtual void AcceptChanges()
        {
            AcceptChanges(ChangeType.Measure);
            AcceptChanges(ChangeType.Arrange);
        }

        public abstract Boolean IsChanged { get; }

    }
}
