using System;
using System.Collections.Generic;

namespace Das.Views.Styles
{
    public class NamedStyle : StyleSheet
    {
        public NamedStyle(String name,
                          IEnumerable<IStyleRule> rules)
            : base(rules)
        {
            Name = name;
        }

        public override String ToString()
        {
            return "Style: " + Name + " - " + _rules.Count + " rules";
        }

        public String Name { get; }
    }
}
