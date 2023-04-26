using System;

namespace Das.Views.DataBinding;

public interface IVisualPropertySetter
{
   String PropertyName {get; }

   Object? GetSourceValue(Object? dataContext);
}