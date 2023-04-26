using Das.Views.Rendering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Das.Views.Panels;

public interface IVisualCollection<TVisual> : IMeasureAndArrange,
                                              IDisposable
   where TVisual : IVisualElement
{
   Int32 Count { get; }
        
   IEnumerable<TVisual> GetAllChildren();

   Boolean Contains(TVisual element);

   Boolean IsTrueForAnyChild<TInput>(TInput input,
                                     Func<TVisual, TInput, Boolean> action);

   Boolean IsTrueForAnyChild(Func<TVisual, Boolean> action);

   IEnumerable<T> GetFromEachChild<T>(Func<TVisual, T> action);

   void RunOnEachChild(Action<TVisual> action);

   void RunOnEachChild<TInput>(TInput input,
                               Action<TInput, TVisual> action);

   Task RunOnEachChildAsync<TInput>(TInput input,
                                    Func<TInput, TVisual, Task> action);
}