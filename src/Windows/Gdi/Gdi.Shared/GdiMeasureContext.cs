using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Das.Gdi.Core;
using Das.Views;
using Das.Views.Colors;
using Das.Views.Controls;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Measuring;
using Das.Views.Rendering;

namespace Das.Gdi
{
    public class GdiMeasureContext : BaseMeasureContext
    {
        public GdiMeasureContext(IVisualSurrogateProvider surrogateProvider,
                                 Dictionary<IVisualElement, ValueSize> lastMeasurements,
                                 IThemeProvider themeProvider,
                                 IVisualLineage visualLineage,
                                 ILayoutQueue layoutQueue)
        : base(surrogateProvider, lastMeasurements, 
            themeProvider, visualLineage, layoutQueue)
        {
            var bmp = new Bitmap(1, 1);
            Graphics = Graphics.FromImage(bmp);
        }


        // ReSharper disable once MemberCanBePrivate.Global
        public Graphics Graphics { get; }


        public override ValueSize MeasureString(String s, IFont font)
        {
            var useFont = GdiTypeConverter.GetFont(font);
            var sz = Graphics.MeasureString(s, useFont);
            return new ValueSize(sz.Width, sz.Height);
        }
    }
}