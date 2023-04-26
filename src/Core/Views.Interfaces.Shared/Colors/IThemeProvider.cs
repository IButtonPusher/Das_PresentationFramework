using System;
using Das.Views.Core.Drawing;

namespace Das.Views.Colors;

public interface IThemeProvider
{
   IColorPalette ColorPalette { get; }

   Boolean IsDarkTheme { get; }
}