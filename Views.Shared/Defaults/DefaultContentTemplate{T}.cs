using System;
using Das.Views.Controls;
using Das.Views.Core.Enums;
using Das.Views.DataBinding;
using Das.Views.Rendering;

namespace Das.Views.Defaults
{
    public class DefaultContentTemplate<T> : DefaultContentTemplate
    {
        public DefaultContentTemplate(IVisualBootstrapper visualBootstrapper, 
                                      IBindableElement<T>? host) : base(visualBootstrapper, host)
        {
        }

        protected override IVisualElement GetToStringLabel(Object dataContext)
        {
            if (!(dataContext is T valid))
                throw new InvalidCastException(dataContext + " cannot cast to " + typeof(T));
              
            var txt = new Label<T>(
                new ObjectBinding<T>(valid),
                _visualBootstrapper)
            {
                HorizontalAlignment = HorizontalAlignments.Center, 
                VerticalAlignment = VerticalAlignments.Center
            };

            return txt;
        }
    }
}
