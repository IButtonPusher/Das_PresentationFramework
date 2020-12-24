using System;
using System.Collections.Generic;

namespace Das.Views.Styles
{
    public interface IStyleSelector
    {
        Boolean TryGetClassName(out String className);
        
        /// <summary>
        /// Returns whether a visual meets all the requirements
        /// </summary>
        Boolean IsSelectVisual(IVisualElement visual);
    }
}
