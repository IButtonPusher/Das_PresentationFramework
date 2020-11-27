using System;
using Das.Views.Core.Drawing;

namespace Das.Views.Styles
{
    public class ColorPalette : IColorPalette
    {
        public ColorPalette(IColor accent, 
                            IColor background)
        {
            Accent = accent;
            Background = background;
        }

        public IColor Accent { get; }

        public IColor Background { get; }
    }
}
