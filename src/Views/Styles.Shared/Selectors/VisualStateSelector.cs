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
            StateType = stateType;
        }

        public IStyleSelector BaseSelector { get; }

        public sealed override Boolean Equals(IStyleSelector other)
        {
            return other is VisualStateSelector stateSelector &&
                   Equals(stateSelector.BaseSelector, BaseSelector) && 
                   stateSelector.StateType == StateType;
        }

        public sealed override Boolean IsFilteringOnVisualState()
        {
            return true;
        }

        public override String ToString()
        {
            return BaseSelector + ":" + StateType;
        }

        public VisualStateType StateType { get; }
    }
}