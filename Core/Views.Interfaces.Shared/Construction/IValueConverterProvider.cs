using Das.Views.DataBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Das.Views.Construction
{
    public interface IValueConverterProvider
    {
        IValueConverter? GetDefaultConverter(Type visualPropertyType);
    }
}
