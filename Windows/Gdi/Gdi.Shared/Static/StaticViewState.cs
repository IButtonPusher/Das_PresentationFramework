using System;
using Das.Views;
using Das.Views.Core.Drawing;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Gdi.Shared.Static
{
    public readonly struct StaticViewState : IViewState
    {
        public StaticViewState(Double zoomLevel)
        {
            _defaultStyle = DefaultStyle.Instance;
            ZoomLevel = zoomLevel;
            ColorPalette = new DefaultColorPalette();
        }

        public T GetStyleSetter<T>(StyleSetter setter, 
                                   IVisualElement element)
        {
            if (_defaultStyle[setter] is T good)
            {
                return good;
            }

            return default!;
        }

        public T GetStyleSetter<T>(StyleSetter setter, 
                                   StyleSelector selector, 
                                   IVisualElement element)
        {
            return GetStyleSetter<T>(setter, element);
        }

        public void RegisterStyleSetter(IVisualElement element, 
                                        StyleSetter setter, 
                                        Object value)
        {
            throw new NotSupportedException();
        }

        public void RegisterStyleSetter(IVisualElement element, 
                                        StyleSetter setter, 
                                        StyleSelector selector, 
                                        Object value)
        {
            throw new NotSupportedException();
        }

        public IColorPalette ColorPalette { get; }

        public IColor GetCurrentAccentColor()
        {
            return ColorPalette.Accent;
        }

        public Double ZoomLevel { get; }

        private readonly DefaultStyle _defaultStyle;
    }
}
