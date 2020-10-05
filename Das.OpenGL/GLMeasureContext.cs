using Das.Views.Measuring;
using System;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;

namespace Das.OpenGL
{
    public class GLMeasureContext : BaseMeasureContext
    {
        public GLMeasureContext(IFontProvider fontProvider)
        {
            _fontProvider = fontProvider;
            
        }

        private readonly IFontProvider _fontProvider;
        
        

        public override Size MeasureString(String s, Font font)
        {
            var renderer = _fontProvider.GetRenderer(font);
            var res = renderer.MeasureString(s);
            
            return res;
        }


        public override Size MeasureImage(IImage img) 
            => throw new NotImplementedException();
    }
}
