using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views.DataBinding;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public interface IVisualContainer : IBindableElement,
                                        IVisualFinder,
                                        IChangeTracking
    {
        //IList<IVisualElement> Children { get; }

        IVisualCollection Children { get; }

        //Boolean IsTrueForAnyChild<TInput>(TInput input,
        //                                  Func<IVisualElement, TInput, Boolean> action);

        //Boolean IsTrueForAnyChild(Func<IVisualElement, Boolean> action);

        //IEnumerable<T> GetFromEachChild<T>(Func<IVisualElement, T> action);

        //void RunOnEachChild(Action<IVisualElement> action);

        //void RunOnEachChild<TInput>(TInput input,
        //                            Action<TInput, IVisualElement> action);

        //Task RunOnEachChildAsync<TInput>(TInput input,
        //                            Func<TInput, IVisualElement, Task> action);

        void AddChild(IVisualElement element);

        Boolean RemoveChild(IVisualElement element);

        void AddChildren(params IVisualElement[] elements);

        void OnChildDeserialized(IVisualElement element,
                                 INode node);
    }
}