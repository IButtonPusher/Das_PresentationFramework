using System;
using Das.Views.DataBinding;

namespace Das.Views.Templates
{
    public interface IDataTemplate<TDataContext> : IDataTemplate
    {
        IBindableElement<TDataContext>? BuildVisual(TDataContext dataContext);

        TVisualElement BuildVisual<TVisualElement>(TDataContext dataContext)
            where TVisualElement : IBindableElement<TDataContext>;
    }
}
