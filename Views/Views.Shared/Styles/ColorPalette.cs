using System;
using Das.Views.Core.Drawing;

namespace Das.Views.Styles
{
    public class ColorPalette : IColorPalette
    {
        public ColorPalette(IBrush accent, 
                            IBrush background)
        {
            Accent = accent;
            Background = background;
        }

        public IBrush Accent { get; }

        public IBrush Background { get; }
    }
}
