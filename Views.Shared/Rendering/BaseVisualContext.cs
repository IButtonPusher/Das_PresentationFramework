using System;
using Das.Views.DataBinding;
using Das.Views.Styles;

namespace Das.Views.Rendering
{
    public abstract class BaseVisualContext : IVisualContext
    {
        public IViewPerspective Perspective { get; }

        private readonly IStyleContext _styleContext;
        protected Object RootDataContext { get; set; }

        private readonly IVisualElement _rootVisualElement;

        public BaseVisualContext(IStyleContext styleContext,
            IVisualElement rootVisualElement, IViewPerspective perspective)
        {
            _styleContext = styleContext;
            _rootVisualElement = rootVisualElement;
            Perspective = perspective;
        }

        public T GetStyleSetter<T>(StyleSetters setter, IVisualElement element)
            => _styleContext.GetStyleSetter<T>(setter, element);

        protected void InvalidateBindings()
        {
            var currentElement = _rootVisualElement;
            var currentDataContext = RootDataContext;

            if (currentElement is IBindingSetter container)
                container.SetBoundValue(currentDataContext);
        }
    }
}