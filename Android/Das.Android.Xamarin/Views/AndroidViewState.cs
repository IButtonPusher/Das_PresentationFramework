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

        public T GetStyleSetter<T>(StyleSetterType setterType,
                                   IVisualElement element)
        {
            return _styleContext.GetStyleSetter<T>(setterType, element);
        }

        public void RegisterStyleSetter(IVisualElement element, 
                                        StyleSetterType setterType, 
                                        Object value)
        {
            _styleContext.RegisterStyleSetter(element, setterType, value);
        }

        public T GetStyleSetter<T>(StyleSetterType setterType,
                                   StyleSelector selector,
                                   IVisualElement element)
        {
            return _styleContext.GetStyleSetter<T>(setterType, selector, element);
        }

        public void RegisterStyleSetter(IVisualElement element, 
                                        StyleSetterType setterType, 
                                        StyleSelector selector, 
                                        Object value)
        {
            _styleContext.RegisterStyleSetter(element, setterType, selector, value);
        }

        public IColorPalette ColorPalette => _styleContext.ColorPalette;

        public Double ZoomLevel { get; }

        
        private readonly IStyleContext _styleContext;
    }
}