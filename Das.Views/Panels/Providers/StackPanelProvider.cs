using System;
using System.Collections.Generic;
using Das.Views.Core.Enums;
using Das.Views.DataBinding;

namespace Das.Views.Panels.Providers
{
    public class StackPanelProvider
    {
        public static IVisualContainer GetStackPanel<TViewModel, TProperty>(Orientations orientation,
            String propertyName)
        {
            var prop = typeof(TViewModel).GetProperty(propertyName);
            if (prop == null)
                throw new InvalidOperationException();

            var isCollection = typeof(IEnumerable<TProperty>).IsAssignableFrom(prop.PropertyType);

            if (isCollection)
            {
                var binding = new DeferredPropertyBinding<IEnumerable<TProperty>>(prop);
                return new StackPanel<IEnumerable<TProperty>>
                    {Binding = binding, Orientation = orientation};
            }
            else
            {
                var binding = new DeferredPropertyBinding<TProperty>(prop);
                return new StackPanel<TProperty>
                    {Binding = binding, Orientation = orientation};
            }
        }
    }
}