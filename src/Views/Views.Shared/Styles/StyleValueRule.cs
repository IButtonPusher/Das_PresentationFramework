using System;
using System.Collections.Generic;
using System.Linq;

namespace Das.Views.Styles
{
    public class StyleValueRule : IStyleRule
    {
        public StyleValueRule(IStyleSelector selector, 
                              IEnumerable<IStyleDeclaration> declarations)
        {
            Selector = selector;
            Declarations = declarations.ToArray();
        }

        public IStyleSelector Selector { get; }

        IEnumerable<IStyleDeclaration> IStyleRule.Declarations => Declarations;
        
        public IStyleDeclaration[] Declarations { get; }

        public override String ToString()
        {
            return Selector + " - " + Declarations.Length + " declarations";
        }
    }
}
