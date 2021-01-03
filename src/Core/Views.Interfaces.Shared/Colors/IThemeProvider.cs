using System;
using System.Collections.Generic;
using Das.Views.Core.Drawing;

namespace Das.Views.Colors
{
    public interface IThemeProvider
    {
        IColorPalette ColorPalette { get; }
    }
}
