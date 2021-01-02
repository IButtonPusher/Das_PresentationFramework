using System;
using System.Threading.Tasks;
using Das.Views.Transitions;

namespace Das.Views.Styles.Transitions
{
    public class DoublePropertyTransition : PropertyTransition<Double>
    {
        public DoublePropertyTransition(IVisualElement visual,
                                        IDependencyProperty<Double> property,
                                        TimeSpan duration,
                 TimeSpan delay,
                 TransitionFunctionType timing)
            : base(visual, property, duration, delay, timing)
        {
        }

        protected override Double GetCurrentValue(Double runningPct)
        {
            return CurrentStartValue + CurrentEndValue * runningPct;
        }

    }
}
