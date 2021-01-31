using System;
using System.Threading.Tasks;
using Das.Views.Colors;
using Das.Views.Core.Geometry;

namespace Das.Views.Rendering
{
    public interface IVisualContext : IThemeProvider
    {
        Double ZoomLevel { get; }

        ValueSize GetLastMeasure(IVisualElement element);
        
        IVisualLineage VisualLineage { get; }

        ILayoutQueue LayoutQueue { get; }

        /// <summary>
        /// Returns the Size + Width properties of the visual, if set.
        /// </summary>
        Boolean TryGetElementSize(IVisualElement visual,
                                  ISize availableSize,
                                  out ValueSize size);
    }
}