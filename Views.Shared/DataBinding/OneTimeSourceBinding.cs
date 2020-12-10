using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Das.Views.DataBinding
{
    public class OneTimeSourceBinding : SourceBinding
    {

        public OneTimeSourceBinding(Object source,
                                    String sourceProperty,
                                    IBindableElement target,
                                    String targetProperty,
                                    IValueConverter? converter) 
            : base(source, sourceProperty, target, targetProperty,converter)
        {
            //_value = value;
        }

        public OneTimeSourceBinding(Object? source,
                                    PropertyInfo srcProp,
                                    IBindableElement target,
                                    PropertyInfo targetProp,
                                    IValueConverter? converter)
            : base(source, srcProp, target, targetProp, converter)
        {
        }


      
    }
}
