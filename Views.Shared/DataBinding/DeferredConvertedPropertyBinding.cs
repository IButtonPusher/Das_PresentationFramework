using System;
using System.Collections.Generic;
using System.Text;

namespace Das.Views.DataBinding
{
    public class DeferredConvertedPropertyBinding : DeferredPropertyBinding
    {
        private readonly IValueConverter _converter;

        public DeferredConvertedPropertyBinding(String sourcePropertyName, 
                                                String targetPropertyName,
                                                IValueConverter converter) 
            : base(sourcePropertyName, targetPropertyName)
        {
            _converter = converter;
        }
    }
}
