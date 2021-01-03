using System;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views.Core.Drawing;
using Das.Views.DataBinding;
using Das.Views.Panels;
using Das.Views.Styles;

namespace Das.Views
{
    public interface IVisualBootstrapper : IPropertyProvider
    {
        //IStyleContext StyleContext { get; }
        IColorPalette ColorPalette { get; }

        IUiProvider UiProvider { get; }

        ILayoutQueue LayoutQueue { get; }


        IVisualElement Instantiate(Type type);

        TVisualElement Instantiate<TVisualElement>(Type type)
            where TVisualElement : IVisualElement;

        TVisualElement Instantiate<TVisualElement>()
            where TVisualElement : IVisualElement;

        /// <summary>
        ///     Instantiates a new instance of the provided visual,
        ///     using the supplied data context, if any.  Useful for runtime types
        /// </summary>
        IBindableElement InstantiateCopy<TVisualElement>(TVisualElement visual,
                                                         Object? dataContext)
            where TVisualElement : IBindableElement;

        TVisualElement InstantiateCopy<TVisualElement>(TVisualElement visual)
            where TVisualElement : IVisualElement;

        IVisualElement InstantiateCopy(IVisualElement visual,
                                       Object? dataContext);

        void ResolveTo<TViewModel, TView>()
            where TView : IView;

        IDataTemplate? TryResolveFromContext(Object dataContext);
    }
}