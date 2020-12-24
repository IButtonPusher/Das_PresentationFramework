using System;
using Das.Views.Core.Drawing;

namespace Das.Views.Styles
{
    public class ColorPalette : IColorPalette
    {
        public ColorPalette(IBrush primary, 
                            IBrush onPrimary,
                            IBrush onSurface, 
                            IBrush background, 
                            IBrush accent)
        {
            Accent = accent;
            Background = background;
            Primary = primary;
            OnSurface = onSurface;
            OnPrimary = onPrimary;
        }

        public IBrush Primary { get; }

        public IBrush Accent { get; }

        public IBrush Background { get; }

        public IBrush OnSurface { get; }

        public IBrush OnPrimary { get; }
    }
}
