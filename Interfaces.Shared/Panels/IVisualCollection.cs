using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
    public interface IVisualCollection : IMeasureAndArrange,
                                         IDisposable
    {
        Int32 Count { get; }

        Boolean Contains(IVisualElement element);

        IEnumerable<IVisualElement> GetAllChildren();

        Boolean IsTrueForAnyChild<TInput>(TInput input,
                                          Func<IVisualElement, TInput, Boolean> action);

        Boolean IsTrueForAnyChild(Func<IVisualElement, Boolean> action);

        IEnumerable<T> GetFromEachChild<T>(Func<IVisualElement, T> action);

        void RunOnEachChild(Action<IVisualElement> action);

        void RunOnEachChild<TInput>(TInput input,
                                    Action<TInput, IVisualElement> action);

        Task RunOnEachChildAsync<TInput>(TInput input,
                                         Func<TInput, IVisualElement, Task> action);
    }

   
}
