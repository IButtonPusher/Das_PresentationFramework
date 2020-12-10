using System;
using System.Collections.Generic;
using Das.Views.Rendering;

namespace Das.Views
{
    public interface IMultiDataTemplate : IDataTemplate
    {
        IEnumerable<IVisualElement> BuildVisuals(Object? dataContext);
    }
}
