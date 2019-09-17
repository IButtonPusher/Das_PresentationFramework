using System.Collections.Generic;
using Das.Serializer;
using Das.Views.DataBinding;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public interface IVisualContainer : IBindableElement, IVisualFinder
    {
        IList<IVisualElement> Children { get; }

        void AddChild(IVisualElement element);

        void AddChildren(params IVisualElement[] elements);

        void OnChildDeserialized(IVisualElement element, INode node);

        
    }
}