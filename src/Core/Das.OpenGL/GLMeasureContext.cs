﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Measuring;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.OpenGL
{
    public class GLMeasureContext : BaseMeasureContext
    {
        public GLMeasureContext(IFontProvider fontProvider,
                                IVisualSurrogateProvider surrogateProvider,
                                Dictionary<IVisualElement, ValueSize> lastMeasurements,
                                IStyleContext styleContext,
                                IVisualLineage visualLineage)
        : base(surrogateProvider, lastMeasurements,styleContext, visualLineage)
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