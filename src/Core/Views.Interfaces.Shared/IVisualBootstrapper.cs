using System;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views.Colors;
using Das.Views.Core.Drawing;
using Das.Views.DataBinding;

namespace Das.Views
{
    public interface IVisualBootstrapper
    {
        IColorPalette ColorPalette { get; }

        IUiProvider UiProvider { get; }

        IPropertyProvider Properties { get; }

        /// <summary>
        /// Sets values of dependency properties based on applicable setters
        /// </summary>
        void ApplyVisualStyling(IVisualElement visual);

        ILayoutQueue LayoutQueue { get; }

        IThemeProvider ThemeProvider { get; }


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

        IDataTemplate? TryResolveFromContext(Object dataContext);

        Object Resolve(Type type);

        T Resolve<T>();
    }
}