using System;
using Das.Serializer;
using Das.Views.DataBinding;
using Das.Views.Panels;
using Das.Views.Styles;


namespace Das.Views
{
    public interface IVisualBootstrapper : IPropertyProvider
    {
        void ResolveTo<TViewModel, TView>()
            where TView : IView;

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
        IBindableElement InstantiateCopy<TVisualElement>(TVisualElement visual,
                                                       Object? dataContext)
            where TVisualElement : IBindableElement;
        
        TVisualElement InstantiateCopy<TVisualElement>(TVisualElement visual)
            where TVisualElement : IVisualElement;

        IVisualElement InstantiateCopy(IVisualElement visual, 
                                       Object? dataContext);
        
        //TVisualElement InstantiateCopy<TVisualElement, TViewModel>(TVisualElement visual,
        //                                                           Object dataContext)
        //    where TVisualElement : IBindableElement;
        

        IUiProvider UiProvider { get; }
    }
}
