using System;

namespace Das.Views
{
    public interface IDataTemplate
    {
        Type? DataType { get; }

        IVisualElement BuildVisual();

        IVisualElement? BuildVisual(Object? dataContext);
    }
}
