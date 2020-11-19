using System;
using Das.Views.Core.Drawing;

namespace Das.Views.Styles
{
    public class ColorPalette : IColorPalette
    {
        public ColorPalette(IColor accent)
        {
            Accent = accent;
        }

        public IColor Accent { get; }
    }
}
