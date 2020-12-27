using System;
using Das.Views.Converters;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;

namespace Das.Views.Construction
{
    public class DefaultValueConverterProvider : IValueConverterProvider
    {
        private readonly StringToVisualConverter _stringToVisualConverter;

        public DefaultValueConverterProvider(IVisualBootstrapper visualBootstrapper)
        {
            _stringToVisualConverter = new StringToVisualConverter(visualBootstrapper);
        }
        
        public IValueConverter? GetDefaultConverter(Type visualPropertyType)
        {
            if (typeof(IBrush).IsAssignableFrom(visualPropertyType))
                return StringToBrushConverter.Instance;

            if (typeof(Thickness).IsAssignableFrom(visualPropertyType))
                return StringToThicknessConverter.Instance;

            if (typeof(IVisualElement).IsAssignableFrom(visualPropertyType))
                return _stringToVisualConverter;

            return default;

        }
    }
}
