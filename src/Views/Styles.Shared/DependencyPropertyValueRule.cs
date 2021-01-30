using System;
using System.Collections.Generic;
using Das.Views.Styles.Declarations;

namespace Das.Views.Styles
{
    public class DependencyPropertyValueRule<TValue> : IStyleRule
    {
        public DependencyPropertyValueRule(IStyleSelector selector, 
                                           ValueDeclaration<TValue> declaration)
        {
            Selector = selector;
            Declaration = declaration;
        }

        public IStyleSelector Selector { get; }

        public IEnumerable<IStyleDeclaration> Declarations
        {
            get
            {
                yield return Declaration;
            }
        }

        public ValueDeclaration<TValue> Declaration { get; }

        public Boolean Equals(IStyleRule other)
        {
            return other is DependencyPropertyValueRule dpvr &&
                   dpvr.Selector.Equals(Selector);
        }

        public override String ToString()
        {
            return Selector + " => " + Declaration;
        }
    }
}
