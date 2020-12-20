using System;
using System.Collections.Generic;
using System.Text;

namespace Das.Views.Rendering
{
    /// <summary>
    /// Allows traversal of the visual hierarchy starting from the visual
    /// currently being rendered up to the root of the visual tree
    /// </summary>
    public interface IVisualLineage : IEnumerable<IVisualElement>
    {
        void PushVisual(IVisualElement visual);

        IVisualElement PopVisual();

        IVisualElement? PeekVisual();
    }
}
