using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.Styles;

namespace Das.Views.Rendering
{
    public interface IVisualContext : IStyleProvider
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