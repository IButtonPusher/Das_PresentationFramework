using System;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Xamarin.Android
{
    public class AndroidStyleProvider : IViewState
    {
        public T GetStyleSetter<T>(StyleSetter setter, IVisualElement element)
        {
            throw new NotImplementedException();
        }

        public T GetStyleSetter<T>(StyleSetter setter, 
                                   StyleSelector selector, 
                                   IVisualElement element)
        {
            throw new NotImplementedException();
        }

        public Double ZoomLevel => 1;
    }
}
