using System;
using Android.Util;
using Das.Views;
using Das.Views.Core.Drawing;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Xamarin.Android
{
    public class AndroidViewState : IViewState
    {
        public AndroidViewState(DisplayMetrics displayMetrics,
                                IStyleContext styleContext)
        {
            
            _styleContext = styleContext;
            ZoomLevel = displayMetrics.ScaledDensity;
        }

        public T GetStyleSetter<T>(StyleSetter setter,
                                   IVisualElement element)
        {
            return _styleContext.GetStyleSetter<T>(setter, element);
        }

        public void RegisterStyleSetter(IVisualElement element, 
                                        StyleSetter setter, 
                                        Object value)
        {
            _styleContext.RegisterStyleSetter(element, setter, value);
        }

        public IColor GetCurrentAccentColor()
        {
            return _styleContext.GetCurrentAccentColor();
        }

        public T GetStyleSetter<T>(StyleSetter setter,
                                   StyleSelector selector,
                                   IVisualElement element)
        {
            return _styleContext.GetStyleSetter<T>(setter, selector, element);
        }

        public void RegisterStyleSetter(IVisualElement element, 
                                        StyleSetter setter, 
                                        StyleSelector selector, 
                                        Object value)
        {
            _styleContext.RegisterStyleSetter(element, setter, selector, value);
        }

        public IColorPalette ColorPalette => _styleContext.ColorPalette;

        public Double ZoomLevel { get; }

        
        private readonly IStyleContext _styleContext;
    }
}