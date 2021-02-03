using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Das.Views.Styles
{
    public class StyleValueRule : IStyleRule
    {
        public StyleValueRule(IStyleSelector selector,
                              IEnumerable<IStyleDeclaration> declarations)
        {
            Selector = selector;
            _hashCode = selector.GetHashCode();
            Declarations = declarations.ToArray();
        }

        public IStyleSelector Selector { get; }

        IEnumerable<IStyleDeclaration> IStyleRule.Declarations => Declarations;

        public Boolean Equals(IStyleRule other)
        {
            return other.Selector.Equals(Selector);
        }

        public IStyleDeclaration[] Declarations { get; }

        public override Boolean Equals(Object obj)
        {
            return obj is IStyleRule dpvr &&
                   dpvr.Selector.Equals(Selector);
        }

        public override Int32 GetHashCode()
        {
            return _hashCode;
        }

        public override String ToString()
        {
            return Selector + " - " + Declarations.Length + " declarations";
        }

        private readonly Int32 _hashCode;
    }
}
