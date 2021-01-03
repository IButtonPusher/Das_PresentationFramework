using System;
using Das.Views.Styles;
using Das.Views.Transitions;

namespace Das.Views
{
   public interface IDependencyProperty<TVisual, TValue>
       where TVisual : IVisualElement
    {
        void AddOnChangedHandler(Action<TVisual, TValue, TValue> handler);

        void AddOnChangedHandler(TVisual forVisual,
                                 Action<TVisual, TValue, TValue> handler);

        void AddOnChangingHandler(TVisual forVisual,
                                  Func<TVisual, TValue, TValue, Boolean> handler);

        void AddTransition(TVisual visual,
                           ITransition<TValue> transition);

        TValue GetValue(TVisual forVisual);

        //TValue GetValue(TVisual forVisual,
        //                IStyleProvider contextStyle,
        //                Func<TVisual, IStyleProvider, TValue> getDefault);

        void SetValue(TVisual forVisual,
                      TValue value,
                      Func<TValue, TValue, Boolean> onChanging,
                      Action<TValue, TValue> onChanged);

        void SetValue(TVisual forVisual,
                      TValue value,
                      Func<TVisual, TValue, TValue, Boolean> onChanging,
                      Action<TVisual, TValue, TValue> onChanged);

        void SetValue(TVisual forVisual,
                      TValue value);

        void SetValueNoTransitions(TVisual forVisual,
                                   TValue value);
    }
}
