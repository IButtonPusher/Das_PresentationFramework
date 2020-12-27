using Das.Views.DataBinding;
using System;

namespace Das.Views.Construction
{
    public interface IValueConverterProvider
    {
        IValueConverter? GetDefaultConverter(Type visualPropertyType);
    }
}
