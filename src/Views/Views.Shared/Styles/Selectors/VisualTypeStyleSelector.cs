using System;
using System.Threading.Tasks;

namespace Das.Views.Styles.Selectors
{
    public class VisualTypeStyleSelector : SelectorBase
    {
        public VisualTypeStyleSelector(Type visualType)
            : base(visualType.GetHashCode())
        {
            VisualType = visualType;
        }

        public sealed override Boolean Equals(IStyleSelector other)
        {
            return other is VisualTypeStyleSelector visType &&
                   visType.VisualType == VisualType;
        }

        public override String ToString()
        {
            return VisualType.Name;
        }

        public Type VisualType { get; }
    }
}