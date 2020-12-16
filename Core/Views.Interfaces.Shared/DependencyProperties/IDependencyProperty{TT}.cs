using Das.Views.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using Das.Views.Styles;

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

        TValue GetValue(TVisual forVisual);

        TValue GetValue(TVisual forVisual,
                        IStyleProvider contextStyle,
                        Func<TVisual, IStyleProvider, TValue> getDefault);

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

        String ToString();
    }
}
