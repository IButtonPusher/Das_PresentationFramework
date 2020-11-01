using System;
using System.Threading.Tasks;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Views
{
    public abstract class ContextBase
    {
        public IViewState? ViewState { get; protected set; }

        public T GetStyleSetter<T>(StyleSetter setter, IVisualElement element)
        {
            return GetViewState().GetStyleSetter<T>(setter, element);
        }

        public T GetStyleSetter<T>(StyleSetter setter, StyleSelector selector, IVisualElement element)
        {
            return GetViewState().GetStyleSetter<T>(setter, selector, element);
        }

        protected IViewState GetViewState()
        {
            return ViewState ??
                   throw new Exception(
                       "No view state has been set.  Call MeasureMainView");
        }
    }
}