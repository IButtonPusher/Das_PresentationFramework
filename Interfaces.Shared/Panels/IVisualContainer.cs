using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views.DataBinding;
using Das.Views.Rendering; //using Das.Serializer;

namespace Das.Views.Panels
{
    public interface IVisualContainer : IBindableElement, IVisualFinder, IChangeTracking
    {
        IList<IVisualElement> Children { get; }

        void AddChild(IVisualElement element);

        void AddChildren(params IVisualElement[] elements);

        void OnChildDeserialized(IVisualElement element, INode node);
    }
}