using System;

namespace Das.Views.Styles.Selectors
{
    public abstract class SelectorBase : IStyleSelector
    {
        protected SelectorBase(Int32 hashCode)
        {
            _hashCode = hashCode;
        }

        public virtual Boolean TryGetClassName(out String className)
        {
            className = default!;
            return false;
        }
        
        public virtual Boolean IsFilteringOnVisualState()
        {
            return false;
        }

        public virtual IStyleSelector ToUnfiltered()
        {
            return this;
        }

        public virtual Boolean TryGetContentAppendType(out ContentAppendType appendType)
        {
            appendType = ContentAppendType.Invalid;
            return false;
        }

        public abstract Boolean Equals(IStyleSelector other);

        public sealed override Int32 GetHashCode() => _hashCode;

        private readonly Int32 _hashCode;
    }
}
