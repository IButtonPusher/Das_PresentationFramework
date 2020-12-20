using System;
using Das.Views.Core.Drawing;

namespace Das.Views.Styles
{
    public class DefaultColorPalette : ColorPalette
    {
        public DefaultColorPalette() 
            : base(SolidColorBrush.Green, SolidColorBrush.White)
        {
        }
    }
}
