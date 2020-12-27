using System;

namespace Das.Views.Styles.Selectors
{
    public abstract class SelectorBase : IStyleSelector
    {
        public virtual Boolean TryGetClassName(out String className)
        {
            className = default!;
            return false;
        }
        
        public virtual Boolean IsFilteringOnVisualState()
        {
            return false;
        }
        
        public virtual Boolean TryGetContentAppendType(out ContentAppendType appendType)
        {
            appendType = ContentAppendType.Invalid;
            return false;
        }

        public abstract Boolean Equals(IStyleSelector other);
    }
}
