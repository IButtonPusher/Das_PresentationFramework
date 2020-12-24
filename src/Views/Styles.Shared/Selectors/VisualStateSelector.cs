using System;

namespace Das.Views.Styles.Selectors
{
    public class VisualStateSelector : SelectorBase,
                                       IStyleSelector
    {
        private readonly VisualStateType _stateType;

        public VisualStateSelector(VisualStateType stateType)
        {
            _stateType = stateType;
        }

        public Boolean IsSelectVisual(IVisualElement visual)
        {
            if (!(visual is IStatefulVisual stateful))
                return false;

            return stateful.CurrentVisualStateType.Contains(_stateType);
        }

        public override String ToString()
        {
            return "Select: " + _stateType;
        }
    }
}
