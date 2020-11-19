using System;
using Das.Views.DataBinding;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Views
{
    public interface IVisualBootStrapper
    {
        void ResolveTo<TViewModel, TView>()
            where TView : IView<TViewModel>;

        IView? TryResolve(Object dataContext);

        IVisualElement Instantiate(Type type,
                                   Int32 styleId);

        IStyleContext StyleContext { get; }

        TVisualElement Instantiate<TVisualElement>(Type type)
            where TVisualElement : IVisualElement;

        TVisualElement Instantiate<TVisualElement>()
            where TVisualElement : IVisualElement;

        IBindableElement Instantiate(Type type,
                                     Object dataContext);
    }
}
