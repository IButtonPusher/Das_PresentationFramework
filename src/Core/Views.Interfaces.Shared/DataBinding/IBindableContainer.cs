using System;

namespace Das.Views.DataBinding;

public interface IBindableContainer : IBindableElement
{
   void UpdateContentDataContext(Object? newValue);
}