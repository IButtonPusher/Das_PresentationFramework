using System;
using Das.Views.Rendering;

namespace Das.Views.Styles
{
    public interface IStyleProvider
    {
        T GetStyleSetter<T>(StyleSetters setter, IVisualElement element);
    }
}