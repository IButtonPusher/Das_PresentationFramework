using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Das.Views.DataBinding
{
    public class ConvertedSourceBinding : SourceBinding
    {
        public ConvertedSourceBinding(INotifyPropertyChanged source, 
                                      String sourceProperty, 
                                      IBindableElement target, 
                                      String targetProperty,
                                      IValueConverter? valueConverter) 
            : base(source, sourceProperty, target, targetProperty, valueConverter)
        {
        }

        public ConvertedSourceBinding(INotifyPropertyChanged source, 
                                      PropertyInfo srcProp,
                                      IBindableElement target,
                                      PropertyInfo targetProp,
                                      IValueConverter? valueConverter) 
            : base(source, srcProp, target, targetProp, valueConverter)
        {
        }
    }
}
