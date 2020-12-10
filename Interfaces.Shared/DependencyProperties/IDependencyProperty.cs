using System;
using System.Collections.Generic;
using System.Text;
using Das.Views.Rendering;

namespace Das.Views
{
    public interface IDependencyProperty
    {
        Object? GetValue(IVisualElement visual);

        void SetValue(IVisualElement visual,
                      Object? value);
    }
}
