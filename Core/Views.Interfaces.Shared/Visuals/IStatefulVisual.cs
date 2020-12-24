using System;
using System.Collections.Generic;
using System.Text;
using Das.Views.Styles;

namespace Das.Views
{
    public interface IStatefulVisual
    {
        VisualStateType CurrentVisualStateType { get; }
    }
}
