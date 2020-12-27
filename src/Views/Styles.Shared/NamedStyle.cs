using System;
using System.Collections.Generic;

namespace Das.Views.Styles
{
    public class NamedStyle : StyleSheet
    {
        public NamedStyle(String name,
                          Type targetType,
                          IEnumerable<IStyleRule> rules)
            : base(rules)
        {
            Name = name;
            TargetType = targetType;
        }

        public override String ToString()
        {
            return "Style: " + Name + " target: " + TargetType.Name + " - " + _rules.Count + " rules";
        }

        public String Name { get; }

        public Type TargetType { get; }
    }
}
