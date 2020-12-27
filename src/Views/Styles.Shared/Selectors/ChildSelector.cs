using System;

namespace Das.Views.Styles.Selectors
{
    public class ChildSelector : SelectorBase
    {
        public IStyleSelector Parent{ get; }

        public IStyleSelector Child { get; }

        public ChildSelector(IStyleSelector parentSelector,
            IStyleSelector selectorForChild)
        {
            Parent = parentSelector;
            Child = selectorForChild;
        }

        public override Boolean Equals(IStyleSelector other)
        {
            return other is ChildSelector childSelector &&
                   Equals(childSelector.Parent, Parent) && 
                   Equals(childSelector.Child, Child);
        }

        public override String ToString()
        {
            return Parent + " > " + Child;
        }
    }
}
