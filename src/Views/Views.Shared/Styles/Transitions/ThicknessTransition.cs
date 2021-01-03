#if !NET40
using TaskEx = System.Threading.Tasks.Task;
#endif

using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.Styles.Transitions;
using Das.Views.Transitions;

namespace Das.Views.Styles
{
    public class ThicknessTransition : PropertyTransition<QuantifiedThickness>
    {
        public ThicknessTransition(IVisualElement visual,
                                   IDependencyProperty<QuantifiedThickness> property,
                                   TimeSpan duration,
                                   TimeSpan delay,
                                   TransitionFunctionType timing) 
            : base(visual, property, duration, delay, timing)
        {
        }


        protected override QuantifiedThickness GetCurrentValue(Double runningPct)
        {
            return CurrentStartValue.Transition(CurrentEndValue, runningPct);
        }

    }
}
