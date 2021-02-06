using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views;
using Das.Views.Colors;
using Das.Views.Controls;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Images;
using Das.Views.Measuring;
using Das.Views.Rendering;

namespace Das.OpenGL
{
    public class GLMeasureContext : BaseMeasureContext
    {
        public GLMeasureContext(IFontProvider fontProvider,
                                IVisualSurrogateProvider surrogateProvider,
                                Dictionary<IVisualElement, ValueSize> lastMeasurements,
                                IThemeProvider themeProvider,
                                IVisualLineage visualLineage,
                                ILayoutQueue layoutQueue)
        : base(surrogateProvider, lastMeasurements, 
            themeProvider, visualLineage, layoutQueue)
        {
            _fontProvider = fontProvider;
        }


        public override ValueSize MeasureImage(IImage img)
        {
            throw new NotImplementedException();
        }


        public override ValueSize MeasureString(String s,
                                                IFont font)
        {
            var renderer = _fontProvider.GetRenderer(font);
            var res = renderer.MeasureString(s);

            return res;
        }

        private readonly IFontProvider _fontProvider;
    }
}