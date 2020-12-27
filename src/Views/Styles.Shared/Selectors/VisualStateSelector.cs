using System;
using System.Threading.Tasks;

namespace Das.Views.Styles.Selectors
{
    public class VisualStateSelector : SelectorBase
    {
        public VisualStateSelector(IStyleSelector selector,
                                   VisualStateType stateType)
        {
            BaseSelector = selector;
            _stateType = stateType;
        }

        public IStyleSelector BaseSelector { get; }

        public sealed override Boolean Equals(IStyleSelector other)
        {
            return other is VisualStateSelector stateSelector &&
                   Equals(stateSelector.BaseSelector, BaseSelector) && 
                   stateSelector._stateType == _stateType;
        }

        public sealed override Boolean IsFilteringOnVisualState()
        {
            return true;
        }

        public override String ToString()
        {
            return BaseSelector + ":" + _stateType;
        }

        private readonly VisualStateType _stateType;
    }
}