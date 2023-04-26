using System;
using System.Collections.Generic;

namespace Das.Views;

public interface IMultiDataTemplate : IDataTemplate
{
   IEnumerable<IVisualElement> BuildVisuals(Object? dataContext);

   IEnumerable<IVisualElement> BuildVisuals();
}