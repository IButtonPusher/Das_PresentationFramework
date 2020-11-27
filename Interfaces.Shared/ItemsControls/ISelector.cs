using System;
using System.Collections.Generic;
using System.Text;
using Das.Views.Rendering;

namespace Das.Views
{
    public interface ISelector : IVisualElement
    {
        Object? SelectedItem { get; set; }

        IVisualElement? SelectedVisual { get; set; }
    }
}
