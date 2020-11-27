using System;
using Das.Container;
using Das.Views.DataBinding;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Views.Templates
{
    public class DefaultVisualBootStrapper : IVisualBootStrapper
    {
        private readonly IResolver _dependencyResolver;
        private IUiProvider? _uiProvider;

        public DefaultVisualBootStrapper(IResolver dependencyResolver,
                                         IStyleContext styleContext)
        {
            StyleContext = styleContext;
            _dependencyResolver = dependencyResolver;
        }

        public void ResolveTo<TViewModel, TView>() 
            where TView : IView<TViewModel>
        {
            throw new NotImplementedException();
        }

        public IVisualElement? TryResolveFromContext(Object dataContext)
        {
            return null;
        }

        public IVisualElement Instantiate(Type type,
                                          Int32 styleId)
        {
            throw new NotImplementedException();
        }

        public IStyleContext StyleContext { get; }

        public TVisualElement Instantiate<TVisualElement>(Type type) 
            where TVisualElement : IVisualElement
        {
            throw new NotImplementedException();
        }

        public TBindableElement Instantiate<TBindableElement>(Type type, 
                                                              IDataBinding? binding)
            where TBindableElement : IBindableElement
        {
            throw new NotImplementedException();
        }

        public TVisualElement Instantiate<TVisualElement>() 
            where TVisualElement : IVisualElement
        {
            throw new NotImplementedException();
        }

        public IBindableElement Instantiate(Type type, 
                                            Object dataContext)
        {
            throw new NotImplementedException();
        }

        public IUiProvider UiProvider => _uiProvider ??=  _dependencyResolver.Resolve<IUiProvider>();
    }
}
