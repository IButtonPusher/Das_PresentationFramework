using System;
using Das.Views.Rendering;

namespace Das.Views.Construction
{
    public interface IViewBuilder
    {
        TVisualElement Instantiate<TVisualElement>(Type type)
            where TVisualElement : IVisualElement;

        /// <summary>
        /// Instantiates a new instance of the provided visual,
        /// using the supplied data context, if any.  Useful for runtime types
        /// </summary>
        TVisualElement InstantiateCopy<TVisualElement>(TVisualElement visual,
                                                       Object? dataContext)
            where TVisualElement : IVisualElement;
    }
}
