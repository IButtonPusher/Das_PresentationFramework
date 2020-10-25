using System;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Measuring;

namespace Das.OpenGL
{
    public class GLMeasureContext : BaseMeasureContext
    {
        public GLMeasureContext(IFontProvider fontProvider)
        {
            _fontProvider = fontProvider;
        }


        public override Size MeasureImage(IImage img)
        {
            throw new NotImplementedException();
        }


        public override Size MeasureString(String s,
                                           IFont font)
        {
            var renderer = _fontProvider.GetRenderer(font);
            var res = renderer.MeasureString(s);

            return res;
        }

        private readonly IFontProvider _fontProvider;
    }
}