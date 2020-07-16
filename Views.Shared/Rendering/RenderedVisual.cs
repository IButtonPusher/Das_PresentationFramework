using System;
using System.Threading.Tasks;
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

        public override String ToString()
        {
            return Element + "\t" + Position;
        }

        public IVisualElement Element { get; }

        public ICube Position { get; }
    }
}