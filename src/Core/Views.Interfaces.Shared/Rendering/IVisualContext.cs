using System;
using System.Threading.Tasks;
using Das.Views.Colors;
using Das.Views.Core.Geometry;

namespace Das.Views.Rendering
{
    public interface IVisualContext : IThemeProvider,
                                      IZoomLevelAware
    {
       ValueSize GetLastMeasure(IVisualElement element);
        
        IVisualLineage VisualLineage { get; }

        ILayoutQueue LayoutQueue { get; }

        /// <summary>
        /// Returns the Size of the visual, if both height and width were specified
        /// </summary>
        Boolean TryGetElementSize(IVisualElement visual,
                                  ISize availableSize,
                                  out ValueSize size);
    }
}