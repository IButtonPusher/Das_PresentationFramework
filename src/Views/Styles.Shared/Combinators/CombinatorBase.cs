using System;
using System.Threading.Tasks;
using Das.Views.Styles.Selectors;

namespace Das.Views.Styles.Combinators
{
    public abstract class CombinatorBase : SelectorBase
    {
        protected CombinatorBase(Int32 hashCode) : base(hashCode)
        {
        }

        public override Boolean Equals(IStyleSelector other)
        {
            return other.GetType() == GetType();
        }
    }
}
