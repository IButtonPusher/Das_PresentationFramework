using System;
using Android.Util;
using Das.Views.Core.Drawing;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Xamarin.Android
{
    public class AndroidViewState : IViewState
    {
        public AndroidViewState(IView view,
                                DisplayMetrics displayMetrics)
        {
            _view = view;
            ZoomLevel = displayMetrics.ScaledDensity;
        }

        public T GetStyleSetter<T>(StyleSetter setter,
                                   IVisualElement element)
        {
            return _view.StyleContext.GetStyleSetter<T>(setter, element);
        }

        public void RegisterStyleSetter(IVisualElement element, 
                                        StyleSetter setter, 
                                        Object value)
        {
            _view.StyleContext.RegisterStyleSetter(element, setter, value);
        }

        public IColor GetCurrentAccentColor()
        {
            return _view.StyleContext.GetCurrentAccentColor();
        }

        public T GetStyleSetter<T>(StyleSetter setter,
                                   StyleSelector selector,
                                   IVisualElement element)
        {
            return _view.StyleContext.GetStyleSetter<T>(setter, selector, element);
        }

        public void RegisterStyleSetter(IVisualElement element, 
                                        StyleSetter setter, 
                                        StyleSelector selector, 
                                        Object value)
        {
            _view.StyleContext.RegisterStyleSetter(element, setter, selector, value);
        }

        public IColorPalette ColorPalette => _view.StyleContext.ColorPalette;

        public Double ZoomLevel { get; }

        private readonly IView _view;
    }
}