using System;
using System.Collections.Generic;
using System.Text;
using Das.Views.Styles.Selectors;

namespace Das.Views.Styles.Combinators
{
    public class ChildCombinator : CombinatorBase
    {
        public override Boolean Equals(IStyleSelector other)
        {
            return other is ChildCombinator;
        }
    }
}
