using System;
using System.Collections.Generic;
using System.Text;

namespace Das.Views.DataBinding
{
    public interface IBindableContainer : IBindableElement
    {
        void UpdateContentDataContext(Object? newValue);
    }
}
