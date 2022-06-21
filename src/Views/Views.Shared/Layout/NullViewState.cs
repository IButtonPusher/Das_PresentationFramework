using System;
using Das.Views.Core.Drawing;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Views.Layout
{
    public class NullViewState : IViewState
    {
        public static readonly NullViewState Instance = new NullViewState();
        
        private NullViewState()
        {
            
        }
        
        
        public Double ZoomLevel => 1.0;

        public Single Density => 1.0f;

        public IColorPalette ColorPalette => BaselineThemeProvider.Instance.ColorPalette;
    }
}
