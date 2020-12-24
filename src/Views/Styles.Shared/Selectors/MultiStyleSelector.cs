using System;
using System.Collections.Generic;

namespace Das.Views.Styles.Selectors
{
    public abstract class MultiStyleSelector
    {
        public MultiStyleSelector()
        {
            _selectors = new List<IStyleSelector>();
        }
        
        public Boolean TryGetClassName(out String className)
        {
            foreach (var selector in _selectors)
            {
                if (selector.TryGetClassName(out className))
                {
                    return true;
                }
            }

            className = default!;
            return false;
        }
        
        protected readonly List<IStyleSelector> _selectors;
    }
}
