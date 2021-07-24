using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views;
using Das.Views.Colors;
using Das.Views.Core;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Images.Svg;
using Das.Views.Layout;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.OpenGL
{
   public class OpenGLRenderKit : BaseRenderKit,
                                  IRenderKit

   {
      public OpenGLRenderKit(IFontProvider fontProvider,
                             IGLContext glContext,
                             IThemeProvider themeProvider,
                             IImageProvider imageProvider)
         : base(imageProvider, Serializer,
            new SvgPathBuilder(imageProvider, Serializer), null,
            BaselineThemeProvider.Instance)
      {
         var lastMeasurements = new Dictionary<IVisualElement, ValueSize>();
         var lastRender = new Dictionary<IVisualElement, ValueCube>();
         var visualLineage = new VisualLineage();

         MeasureContext = new GLMeasureContext(fontProvider, this,
            lastMeasurements, themeProvider, visualLineage, _layoutQueue);

         RenderContext = new GLRenderContext(new BasePerspective(),
            glContext, fontProvider, this, themeProvider,
            visualLineage, lastRender, _layoutQueue);
      }

      IMeasureContext IRenderKit.MeasureContext => MeasureContext;

      IRenderContext IRenderKit.RenderContext => RenderContext;

      public GLMeasureContext MeasureContext { get; }

      public GLRenderContext RenderContext { get; }

      protected static readonly DasSerializer Serializer = new();
   }
}
