using System;
using System.Threading.Tasks;

namespace Das.Views.Styles.Selectors
{
    public class ChildSelector : SelectorBase
    {
        public ChildSelector(IStyleSelector parentSelector,
                             IStyleSelector selectorForChild)
            : base(parentSelector.GetHashCode() & selectorForChild.GetHashCode())
        {
            Parent = parentSelector;
            Child = selectorForChild;
        }

        public IStyleSelector Child { get; }

        public IStyleSelector Parent { get; }

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
