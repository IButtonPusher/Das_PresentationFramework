using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Styles.Declarations;

namespace Das.Views.Styles
{
    public class DependencyPropertyValueRule<TValue> : IStyleRule
    {
        public DependencyPropertyValueRule(IStyleSelector selector,
                                           ValueDeclaration<TValue> declaration)
        {
            Selector = selector;
            _hashCode = selector.GetHashCode() & declaration.GetHashCode();
            Declaration = declaration;
        }

        public IStyleSelector Selector { get; }

        public IEnumerable<IStyleDeclaration> Declarations
        {
            get { yield return Declaration; }
        }

        public Boolean Equals(IStyleRule other)
        {
            return other is DependencyPropertyValueRule<TValue> dpvr &&
                   dpvr.Selector.Equals(Selector) && 
                   dpvr.Declaration.Equals(Declaration);
        }

        public ValueDeclaration<TValue> Declaration { get; }

        public override Boolean Equals(Object obj)
        {
            return obj is DependencyPropertyValueRule<TValue> dpvr &&
                   dpvr.Selector.Equals(Selector) && 
                   dpvr.Declaration.Equals(Declaration);
        }

        public override Int32 GetHashCode()
        {
            return _hashCode;
        }

        public override String ToString()
        {
            return Selector + " => " + Declaration;
        }

        private readonly Int32 _hashCode;
    }
}
