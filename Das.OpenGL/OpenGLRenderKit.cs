﻿using Das.Views;
using System;
using Das.Views.Core.Writing;
using Das.Views.Rendering;

namespace Das.OpenGL
{
    public class OpenGLRenderKit : IRenderKit
    {
        public OpenGLRenderKit(IFontProvider fontProvider, IGLContext glContext)
        {
            MeasureContext = new GLMeasureContext(fontProvider);
            RenderContext = new GLRenderContext(MeasureContext, new BasePerspective(),
                 glContext, fontProvider);
        }

        IMeasureContext IRenderKit.MeasureContext => MeasureContext;
        IRenderContext IRenderKit.RenderContext => RenderContext;

        public GLMeasureContext MeasureContext { get; }

        public GLRenderContext RenderContext { get; }


    }
}