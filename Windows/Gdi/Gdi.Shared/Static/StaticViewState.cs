using System;
using Das.Views;
using Das.Views.Core.Drawing;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Gdi.Shared.Static
{
    public readonly struct StaticViewState : IViewState
    {
        public StaticViewState(Double zoomLevel,
                               IColorPalette colorPalette)
        {
            _defaultStyle = DefaultStyle.Instance;
            ZoomLevel = zoomLevel;
            ColorPalette = colorPalette;//new DefaultColorPalette();
        }

        public T GetStyleSetter<T>(StyleSetterType setterType, 
                                   IVisualElement element)
        {
            if (_defaultStyle[setterType] is T good)
            {
                return good;
            }

            return default!;
        }

        public T GetStyleSetter<T>(StyleSetterType setterType, 
                                   StyleSelector selector, 
                                   IVisualElement element)
        {
            return GetStyleSetter<T>(setterType, element);
        }

        public void RegisterStyleSetter(IVisualElement element, 
                                        StyleSetterType setterType, 
                                        Object value)
        {
            throw new NotSupportedException();
        }

        public void RegisterStyleSetter(IVisualElement element, 
                                        StyleSetterType setterType, 
                                        StyleSelector selector, 
                                        Object value)
        {
            throw new NotSupportedException();
        }

        public IColorPalette ColorPalette { get; }


        public Double ZoomLevel { get; }

        private readonly DefaultStyle _defaultStyle;
    }
}
