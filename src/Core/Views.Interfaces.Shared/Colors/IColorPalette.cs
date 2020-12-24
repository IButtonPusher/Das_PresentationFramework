using System;

namespace Das.Views.Core.Drawing
{
    public interface IColorPalette
    {
        IBrush Primary { get; }
        
        IBrush Accent { get; }

        IBrush Background { get; }
        
        IBrush OnSurface { get; }
        
        IBrush OnPrimary { get; }
    }
}
