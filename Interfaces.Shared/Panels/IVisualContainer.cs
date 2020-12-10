using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views.DataBinding;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    //public interface IVisualContainer : IVisualContainer<IVisualElement>
    //{
        
    //}

    public interface IVisualContainer : IBindableElement,
                                        IVisualFinder,
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