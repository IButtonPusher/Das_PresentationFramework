using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Das.Serializer;

namespace Das.Views.Panels
{

    public interface IVisualContainer : IVisualFinder,
                                        IChangeTracking,
                                        IPanelElement
    {
        new IVisualCollection Children { get; }

        new void AddChild(IVisualElement element);

        Boolean RemoveChild(IVisualElement element);

        new void AddChildren(IEnumerable<IVisualElement> elements);

        void OnChildDeserialized(IVisualElement element,
                                 INode node);

    }
}