using System;
using System.Collections.Generic;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;

namespace Das.Views.Kits
{
    public class CoreRenderKit : IRenderKit
    {
        public CoreRenderKit(IMeasureContext measureContext, IRenderContext renderContext)
        {
            MeasureContext = measureContext;
            RenderContext = renderContext;
        }

        
        public IMeasureContext MeasureContext { get; }
        public IRenderContext RenderContext { get; }
        
        public IEnumerable<IRenderedVisual> GetElementsAt(IPoint point) => throw new NotImplementedException();
    }
}
