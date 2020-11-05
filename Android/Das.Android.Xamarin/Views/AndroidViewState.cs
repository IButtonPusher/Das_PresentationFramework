using System;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Xamarin.Android
{
    public class AndroidViewState : IViewState
    {
        public AndroidViewState(IView view)
        {
            _view = view;
        }

        public T GetStyleSetter<T>(StyleSetter setter,
                                   IVisualElement element)
        {
            return _view.StyleContext.GetStyleSetter<T>(setter, element);
        }

        public T GetStyleSetter<T>(StyleSetter setter,
                                   StyleSelector selector,
                                   IVisualElement element)
        {
            return _view.StyleContext.GetStyleSetter<T>(setter, selector, element);
        }

        public Double ZoomLevel => 1;

        private readonly IView _view;
    }
}