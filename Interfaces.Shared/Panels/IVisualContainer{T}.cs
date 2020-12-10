using Das.Views.DataBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Das.Serializer;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public interface IVisualContainer<TVisual> : IBindableElement,
                                                 IVisualFinder,
                                                 IChangeTracking,
                                                 IPanelElement
        where TVisual : IVisualElement
    {
        new IVisualCollection<TVisual> Children { get; }

        void AddChild(TVisual element);

        Boolean RemoveChild(TVisual element);

        void AddChildren(IEnumerable<TVisual> elements);

        void OnChildDeserialized(TVisual element,
                                 INode node);
    }
}
