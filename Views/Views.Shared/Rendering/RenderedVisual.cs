using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.Views.Rendering
{
    public readonly struct RenderedVisual : IRenderedVisual
    {
        public RenderedVisual(IVisualElement element,
                              ICube position)
        {
            Element = element;
            Position = position;
        }

        public IVisualElement Element { get; }

        public ICube Position { get; }

        public override String ToString()
        {
            return Element + "\t" + Position;
        }
    }

    public readonly struct RenderedVisual<TElement> : IRenderedVisual<TElement>
        //where TElement : IVisualElement
    {
        public RenderedVisual(TElement element,
                              ICube position)
        {
            Element = element;
            Position = position;
        }

        public TElement Element { get; }

        public ICube Position { get; }
    }
}