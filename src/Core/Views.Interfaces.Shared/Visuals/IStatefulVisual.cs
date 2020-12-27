using System;
using Das.Views.Styles;

namespace Das.Views
{
    public interface IStatefulVisual
    {
        VisualStateType CurrentVisualStateType { get; }
    }
}
