using System;
using System.Threading.Tasks;

namespace Das.Views.Styles.Selectors
{
    public class DependencyPropertySelector : SelectorBase
    {
        public DependencyPropertySelector(IDependencyProperty property)
        : base(property.GetHashCode())
        {
            Property = property;
        }

        public IDependencyProperty Property { get; }

        public sealed override Boolean Equals(IStyleSelector other)
        {
            return other is DependencyPropertySelector depSel &&
                   depSel.Property.Name == Property.Name &&
                   depSel.Property.VisualType == Property.VisualType &&
                   depSel.Property.PropertyType == Property.PropertyType;
        }

        public override IStyleSelector ToUnfiltered()
        {
            return AllStyleSelector.Instance;
        }

        public override String ToString()
        {
            return Property.Name;
        }
    }
}
