using System;
using System.Threading.Tasks;
using Das.Views.Colors;
using Das.Views.Core.Drawing;

namespace Das.Views.Styles
{
   public class BaselineThemeProvider : IThemeProvider
   {
      private BaselineThemeProvider()
      {
         ColorPalette = Styles.ColorPalette.Baseline;
      }


      public IColorPalette ColorPalette { get; }

      public Boolean IsDarkTheme => false;

      public static readonly BaselineThemeProvider Instance = new BaselineThemeProvider();
   }
}
