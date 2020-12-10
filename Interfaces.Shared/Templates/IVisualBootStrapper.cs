using System;
using Das.Views.DataBinding;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Views
{
    public interface IVisualBootstrapper
    {
        void ResolveTo<TViewModel, TView>()
            where TView : IView<TViewModel>;

        IDataTemplate? TryResolveFromContext(Object dataContext);

        IVisualElement Instantiate(Type type);

        IStyleContext StyleContext { get; }

        TVisualElement Instantiate<TVisualElement>(Type type)
            where TVisualElement : IVisualElement;
        
        TVisualElement Instantiate<TVisualElement>()
            where TVisualElement : IVisualElement;

        /// <summary>
        /// Instantiates a new instance of the provided visual,
        /// using the supplied data context, if any.  Useful for runtime types
        /// </summary>
        TVisualElement InstantiateCopy<TVisualElement>(TVisualElement visual,
                                                       Object? dataContext)
            where TVisualElement : IVisualElement;
        
        TVisualElement InstantiateCopy<TVisualElement, TViewModel>(TVisualElement visual,
                                                                   TViewModel dataContext)
            where TVisualElement : IBindableElement<TViewModel>;
        

        IUiProvider UiProvider { get; }
    }
}
