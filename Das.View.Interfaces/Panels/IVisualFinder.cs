using System;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public interface IVisualFinder
    {
        Boolean Contains(IVisualElement element);
    }
}
