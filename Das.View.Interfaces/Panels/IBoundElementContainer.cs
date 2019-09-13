using System;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Views.Panels
{
    public interface IBoundElementContainer
    {
        IVisualElement Element { get; set; }

        IViewModel DataContext { get; set; }

        IStyleContext StyleContext { get; set; }

        Size AvailableSize { get; }
    }
}