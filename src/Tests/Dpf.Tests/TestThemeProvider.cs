using System;
using System.Threading.Tasks;
using Das.Views.Colors;
using Das.Views.Core.Drawing;

namespace Dpf.Tests
{
    public class TestThemeProvider : IThemeProvider
    {
        public IColorPalette ColorPalette => Das.Views.Styles.ColorPalette.Baseline;
    }
}
