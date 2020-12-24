using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Das.Views.Styles.Selectors
{
    public class OrStyleSelector
    {
        public OrStyleSelector(IEnumerable<IStyleSelector> selectors)
        {
            _selectors = new List<IStyleSelector>(selectors);
        }

        public Boolean IsSelectVisual(IVisualElement visual)
        {
            foreach (var selector in _selectors)
                if (selector.IsSelectVisual(visual))
                    return true;

            return false;
        }

        private readonly List<IStyleSelector> _selectors;
    }
}