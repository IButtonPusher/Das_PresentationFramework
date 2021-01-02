using System;
using Das.Views.Core.Drawing;
using Das.Views.Core.Writing;

namespace Das.Views.Primitives
{
    public interface ITextVisual : IVisualElement
    {
        String Text { get; set; }
        
        IBrush? TextBrush { get; set; }

        FontStyle FontWeight { get; set; }

        String FontName { get; set; }

        Double FontSize { get; set; }
    }
}
