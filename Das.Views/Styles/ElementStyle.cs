using System;
using Das.Views.Rendering;

namespace Das.Views.Styles
{
    public class ElementStyle : Style
    {
        public ElementStyle(IVisualElement element)
        {
            Element = element;
        }

        public IVisualElement Element { get; }
    }
}