using System;

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
