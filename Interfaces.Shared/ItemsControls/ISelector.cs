using System;
using Das.Views.Rendering;

namespace Das.Views
{
    public interface ISelector : IVisualElement
    {
        Object? SelectedItem { get; set; }

        IVisualElement? SelectedVisual { get; set; }
    }
}
