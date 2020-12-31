using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Das.Views.Styles
{
    public class NamedTargetedStyle : NamedStyle
    {
        public NamedTargetedStyle(String name,
                                  Type targetType,
                                  IEnumerable<IStyleRule> rules)
            : base(name, rules)
        {
            TargetType = targetType;
        }

        public Type TargetType { get; }

        public override String ToString()
        {
            return "Style: " + Name + " target: " + TargetType.Name + " - " + _rules.Count + " rules";
        }
    }
}