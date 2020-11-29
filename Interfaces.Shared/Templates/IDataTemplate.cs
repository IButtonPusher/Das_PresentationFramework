using System;
using Das.Views.Rendering;

namespace Das.Views
{
    public interface IDataTemplate
    {
        Type? DataType { get; }

        IVisualElement? BuildVisual(Object? dataContext);

        TVisualElement BuildVisual<TVisualElement>(Object? dataContext)
            where TVisualElement : IVisualElement;

        //IVisualElement Template { get; }
    }
}
