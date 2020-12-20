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
    }
}