using System;
using Das.Views.Styles;

namespace Das.Views
{
    public interface IStatefulVisual //: IVisualElement
    {
        VisualStateType CurrentVisualStateType { get; }
    }
}
