using System;
using Das.Views.Core.Drawing;

namespace Das.Views.Primitives
{
    public interface ITextVisual : IFontVisual
    {
        String Text { get; set; }
        
        IBrush? TextBrush { get; set; }
    }
}
