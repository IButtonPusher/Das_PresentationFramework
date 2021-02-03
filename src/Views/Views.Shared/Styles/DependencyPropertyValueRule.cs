using System;
using System.Collections.Generic;
using Das.Views.Styles.Declarations;
using Das.Views.Styles.Selectors;

namespace Das.Views.Styles
{
    public class DependencyPropertyValueRule : IStyleRule
    {
        public DependencyPropertyValueRule(DependencyPropertySelector selector, 
                                           ObjectDeclaration declaration)
        {
            Selector = selector;
            _hashCode = selector.GetHashCode() & declaration.GetHashCode();
            Declaration = declaration;
        }

        public DependencyPropertySelector Selector { get; }

        public IEnumerable<IStyleDeclaration> Declarations
        {
            get
            {
                yield return Declaration;
            }
        }

        public ObjectDeclaration Declaration { get; }

        IStyleSelector IStyleRule.Selector => Selector;

        public Boolean Equals(IStyleRule other)
        {
            return other is DependencyPropertyValueRule dpvr &&
                   dpvr.Selector.Equals(Selector);
        }

        public override Boolean Equals(Object obj)
        {
            return obj is DependencyPropertyValueRule dpvr &&
                   dpvr.Selector.Equals(Selector) &&
                   dpvr.Declaration.Equals(Declaration);
        }

        public override Int32 GetHashCode() => _hashCode;
        

        public override String ToString()
        {
            return Selector + " => " + Declaration;
        }

        private readonly Int32 _hashCode;
    }
}
