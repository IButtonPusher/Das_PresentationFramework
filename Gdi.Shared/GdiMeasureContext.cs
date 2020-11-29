using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Das.Gdi.Core;
using Das.Views.Controls;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Measuring;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Gdi
{
    public class GdiMeasureContext : BaseMeasureContext
    {
        public GdiMeasureContext(IVisualSurrogateProvider surrogateProvider,
                                 Dictionary<IVisualElement, ValueSize> lastMeasurements,
                                 IStyleContext styleContext)
        : base(surrogateProvider, lastMeasurements, styleContext)
        {
            var bmp = new Bitmap(1, 1);
            Graphics = Graphics.FromImage(bmp);
        }


        // ReSharper disable once MemberCanBePrivate.Global
        public Graphics Graphics { get; }


        public override ValueSize MeasureString(String s, IFont font)
        {
            var useFont = TypeConverter.GetFont(font);
            var sz = Graphics.MeasureString(s, useFont);
            return new ValueSize(sz.Width, sz.Height);
        }
    }
}