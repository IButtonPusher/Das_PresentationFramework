using System;
using System.Drawing;
using System.Threading.Tasks;
using Das.Gdi.Core;
using Das.Views.Controls;
using Das.Views.Core.Writing;
using Das.Views.Measuring;
using Size = Das.Views.Core.Geometry.Size;

namespace Das.Gdi
{
    public class GdiMeasureContext : BaseMeasureContext
    {
        public GdiMeasureContext(IVisualSurrogateProvider surrogateProvider)
        : base(surrogateProvider)
        {
            var bmp = new Bitmap(1, 1);
            Graphics = Graphics.FromImage(bmp);
        }


        // ReSharper disable once MemberCanBePrivate.Global
        public Graphics Graphics { get; }


        public override Size MeasureString(String s, IFont font)
        {
            var useFont = TypeConverter.GetFont(font);
            var sz = Graphics.MeasureString(s, useFont);
            return new Size(sz.Width, sz.Height);
        }
    }
}