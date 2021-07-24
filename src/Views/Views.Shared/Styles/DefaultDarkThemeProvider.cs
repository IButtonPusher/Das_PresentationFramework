using System;
using System.Threading.Tasks;
using Das.Views.Colors;
using Das.Views.Core.Drawing;

namespace Das.Views.Styles
{
   public class DefaultDarkThemeProvider : IThemeProvider
   {
      private DefaultDarkThemeProvider()
      {
         ColorPalette = Styles.ColorPalette.DefaultDark;
      }

      public IColorPalette ColorPalette { get; }

      public Boolean IsDarkTheme => true;

      public static readonly DefaultDarkThemeProvider Instance = new DefaultDarkThemeProvider();
   }
}
