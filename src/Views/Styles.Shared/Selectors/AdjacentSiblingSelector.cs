using System;
using System.Threading.Tasks;

namespace Das.Views.Styles.Selectors
{
    /// <summary>
    /// The + combinator selects adjacent siblings. This means that the second element
    /// directly follows the first, and both share the same parent.
    /// <a href="https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_Selectors">Source</a></summary>
    public class AdjacentSiblingSelector : SelectorBase
    {
        public AdjacentSiblingSelector(IStyleSelector firstElementSelector,
                                       IStyleSelector secondElementSelector)
        {
            FirstElementSelector = firstElementSelector;
            SecondElementSelector = secondElementSelector;
        }

        public IStyleSelector FirstElementSelector { get; }

        public IStyleSelector SecondElementSelector { get; }

        public override Boolean Equals(IStyleSelector other)
        {
            throw new NotImplementedException();
        }
    }
}