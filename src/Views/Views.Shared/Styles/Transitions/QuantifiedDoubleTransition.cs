using System;
using System.Threading.Tasks;
using Das.Views.Transitions;

namespace Das.Views.Styles.Transitions
{
    public class QuantifiedDoubleTransition : PropertyTransition<QuantifiedDouble?>
    {
        public QuantifiedDoubleTransition(IVisualElement visual,
                                          IDependencyProperty<QuantifiedDouble?> property,
                                          TimeSpan duration,
                                          TimeSpan delay,
                                          TransitionFunctionType timing)
            : base(visual, property, duration, delay, timing)
        {
            _currentDifference = QuantifiedDouble.Zero;
        }

        public override void SetValue(QuantifiedDouble? startValue,
                                      QuantifiedDouble? endValue)
        {
            _currentDifference = endValue.GetValueOrDefault() - startValue.GetValueOrDefault();
            base.SetValue(startValue, endValue);
        }

        protected override QuantifiedDouble? GetCurrentValue(Double runningPct)
        {
            return _currentDifference * runningPct + CurrentStartValue;
        }

        private QuantifiedDouble _currentDifference;
    }
}