using System;
using Das.Views.Colors;

namespace Das.Views.Core.Drawing
{
    public interface IColorPalette
    {
        IBrush Primary { get; }
        
        IBrush Accent { get; }

        IBrush Background { get; }

        IBrush OnBackground {get;}
        
        IBrush Surface { get; }

        IBrush OnSurface { get; }
        
        IBrush OnPrimary { get; }

        IBrush GetAlpha(ColorType type,
                        Double opacity);
    }
}
