using System;
using Das.Views.Colors;
using Das.Views.Core.Drawing;

namespace Das.Views.Styles
{
    public class BaselineThemeProvider : IThemeProvider
    {
        private BaselineThemeProvider()
        {
            ColorPalette = Das.Views.Styles.ColorPalette.Baseline;
        }

        public static readonly BaselineThemeProvider Instance = new BaselineThemeProvider();


        public IColorPalette ColorPalette { get; }
    }
}
