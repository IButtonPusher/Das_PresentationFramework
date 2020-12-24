using System;
using System.ComponentModel;
using System.Reflection;
using Das.Serializer;

namespace Das.Views.DataBinding
{
    public class ConvertedSourceBinding : SourceBinding
    {
        public ConvertedSourceBinding(INotifyPropertyChanged source, 
                                      String sourceProperty, 
                                      IBindableElement target, 
                                      String targetProperty,
                                      IValueConverter? valueConverter,
                                      IPropertyAccessor sourcePropertyAccessor) 
            : base(source, sourceProperty, target, targetProperty, 
                valueConverter, sourcePropertyAccessor)
        {
        }

        public ConvertedSourceBinding(INotifyPropertyChanged source, 
                                      PropertyInfo srcProp,
                                      IBindableElement target,
                                      PropertyInfo targetProp,
                                      IValueConverter? valueConverter,
                                      IPropertyAccessor sourcePropertyAccessor) 
            : base(source, srcProp, target, targetProp, valueConverter, sourcePropertyAccessor)
        {
        }
    }
}
