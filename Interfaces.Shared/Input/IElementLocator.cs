using System.Collections.Generic;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;

namespace Das.Views.Input
{
    public interface IElementLocator
    {
        IEnumerable<IRenderedVisual> GetElementsAt(IPoint point);
    }
}