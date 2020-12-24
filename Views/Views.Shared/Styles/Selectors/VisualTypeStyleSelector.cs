using System;
using System.Threading.Tasks;

namespace Das.Views.Styles.Selectors
{
    public class VisualTypeStyleSelector : SelectorBase,
                                           IStyleSelector
    {
        public VisualTypeStyleSelector(Type visualType)
        {
            _visualType = visualType;
        }

        public Boolean IsSelectVisual(IVisualElement visual)
        {
            return _visualType.IsInstanceOfType(visual);
        }

        public override String ToString()
        {
            return "Select: " + _visualType.Name;
        }

        private readonly Type _visualType;
    }
}