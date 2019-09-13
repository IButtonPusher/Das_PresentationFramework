using System;
using Das.Views.Core.Geometry;

namespace Das.Views.Rendering
{
    public class RenderedVisual : IRenderedVisual
    {
        public RenderedVisual(IVisualElement element, ICube position)
        {
            Element = element;
            Position = position;
        }

        public IVisualElement Element { get; }
        public ICube Position { get; }

        public override string ToString() => Element + "\t" + Position;
    }
}