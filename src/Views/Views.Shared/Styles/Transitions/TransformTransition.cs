using System;
using Das.Views.Transforms;
using Das.Views.Transitions;

namespace Das.Views.Styles.Transitions
{
    public class TransformTransition : PropertyTransition<TransformationMatrix>
    {
        public TransformTransition(IVisualElement visual,
                 IDependencyProperty<TransformationMatrix> property,
                 TimeSpan duration,
                 TimeSpan delay,
                 TransitionFunctionType timing) 
            : base(visual, property, duration, delay, timing)
        {
        }

        protected override TransformationMatrix GetCurrentValue(Double runningPct)
        {
            var res = CurrentStartValue.Transition(CurrentEndValue, runningPct);
            return res;

        }
    }
}
