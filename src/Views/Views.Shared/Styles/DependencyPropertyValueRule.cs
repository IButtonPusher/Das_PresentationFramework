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

        public override String ToString()
        {
            return Selector + " => " + Declaration;
        }
    }
}
