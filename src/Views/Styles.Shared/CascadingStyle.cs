using System;
using System.Collections.Generic;
using System.Text;

namespace Das.Views.Styles
{
    public class CascadingStyle : ICascadingStyle
    {
        public CascadingStyle(IEnumerable<IStyleRule> rules)
        {
            _rules = new List<IStyleRule>(rules);
        }


        private readonly List<IStyleRule> _rules;

        public IEnumerable<IStyleRule> Rules => _rules;
    }
}
