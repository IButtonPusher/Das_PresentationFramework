using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Das.Views.Styles.Selectors
{
    public class AndStyleSelector : MultiStyleSelector, 
                                    IStyleSelector
    {
        public AndStyleSelector(IEnumerable<IStyleSelector> selectors)
        {
            //_selectors = new List<IStyleSelector>();

            foreach (var selector in selectors)
            {
                if (selector is AndStyleSelector andy)
                {
                    _selectors.AddRange(andy._selectors);
                }
                else
                {
                    _selectors.Add(selector);
                }
            }
            
        }

       

        public Boolean IsSelectVisual(IVisualElement visual)
        {
            foreach (var selector in _selectors)
                if (!selector.IsSelectVisual(visual))
                    return false;

            return true;
        }

        public override String ToString()
        {
            return String.Join(", ", _selectors);
        }

        //private readonly List<IStyleSelector> _selectors;
    }
}