using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Das.Views.Styles.Selectors
{
    public class AndStyleSelector : MultiStyleSelector, 
                                    IStyleSelector
    {
        public AndStyleSelector(IEnumerable<IStyleSelector> selectors)
        {
            BuildAndItems(selectors);
        }

        public AndStyleSelector(params IStyleSelector[] selectors)
        {
            BuildAndItems(selectors);
        }

        public Int32 Count => _selectors.Count;

        public IStyleSelector this[Int32 index] => _selectors[index];

        private void BuildAndItems(IEnumerable<IStyleSelector> selectors)
        {
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

        public Boolean Equals(IStyleSelector other)
        {
            if (!(other is AndStyleSelector andy))
                return false;

            if (andy._selectors.Count != _selectors.Count)
                return false;

            for (var c = 0; c < _selectors.Count; c++)
            {
                if (!_selectors[c].Equals(andy._selectors[c]))
                    return false;
            }

            return true;
        }

        public override String ToString()
        {
            return String.Join(" ", _selectors);
        }

        public Boolean TryGetContentAppendType(out ContentAppendType appendType)
        {
            foreach (var selector in _selectors)
            {
                if (selector.TryGetContentAppendType(out appendType))
                    return true;
            }

            appendType = ContentAppendType.Invalid;
            return false;
        }

        public Boolean IsFilteringOnVisualState()
        {
            return _selectors.Any(s => s.IsFilteringOnVisualState());
        }

        public IStyleSelector ToUnfiltered()
        {
            if (!IsFilteringOnVisualState())
                return this;

            var andy = new AndStyleSelector();

            for (var c = 0; c < _selectors.Count; c++)
            {
                var selector = _selectors[c];

                var unf = selector.ToUnfiltered();

                if (unf == AllStyleSelector.Instance)
                {
                    if (c < _selectors.Count - 1 && _selectors[c + 1] is CombinatorSelector)
                        c++;

                    continue;
                }

                andy._selectors.Add(unf);
            }

            if (andy._selectors.Last() is CombinatorSelector combinator)
            {
                andy._selectors.Remove(combinator);
            }

            return andy._selectors.Count switch
            {
                0 => AllStyleSelector.Instance,
                1 => andy._selectors[0],
                _ => andy
            };
        }
    }
}