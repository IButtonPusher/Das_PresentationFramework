using System;

namespace Das.Views.Styles.Selectors
{
    public abstract class SelectorBase
    {
        public Boolean TryGetClassName(out String className)
        {
            className = default!;
            return false;
        }
    }
}
