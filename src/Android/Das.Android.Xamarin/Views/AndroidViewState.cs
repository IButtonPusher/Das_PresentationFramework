using System;
using Android.Util;
using Das.Views.Colors;
using Das.Views.Core.Drawing;
using Das.Views.Rendering;

namespace Das.Xamarin.Android
{
    public class AndroidViewState : IViewState
    {
        private readonly IThemeProvider _themeProvider;

        public AndroidViewState(DisplayMetrics displayMetrics,
                                IThemeProvider themeProvider)
        {
            _themeProvider = themeProvider;
            Density = displayMetrics.Density;
            ZoomLevel = displayMetrics.ScaledDensity;
        }

        public Double ZoomLevel { get; }

        public Single Density { get; }

        public IColorPalette ColorPalette => _themeProvider.ColorPalette;
    }
}