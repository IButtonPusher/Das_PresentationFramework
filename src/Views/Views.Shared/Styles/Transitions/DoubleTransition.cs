#if !NET40
using TaskEx = System.Threading.Tasks.Task;
#endif

using System;
using System.Threading;
using Das.Views.Rendering;
using System.Threading.Tasks;
using Das.Views.Styles.Transitions;

namespace Das.Views.Styles
{
    public class DoubleTransition : BaseTransition
    {
        private readonly AssignedStyle _assignedStyle;
        private readonly Action<AssignedStyle> _updater;

        private readonly Double _initialValue;
        private readonly Double _valueDifference;

        public DoubleTransition(Object? initialValue,
                                Transition transition,
                                AssignedStyle assignedStyle,
                                Action<AssignedStyle> updater)
            : base(transition.Duration, transition.Delay, Easing.QuadraticOut)
        {
            Double finalValue;
            _initialValue = initialValue is Double d ? d : 0;
            if (assignedStyle.Value is Double valid)
                finalValue = valid;
            else 
                throw new NotImplementedException();

            _valueDifference = finalValue - _initialValue;

            _assignedStyle = assignedStyle;
            _updater = updater;
        }

        public override void Start()
        {
            TaskEx.Run(() => RunUpdates(CancellationToken.None)).ConfigureAwait(false);
        }


        protected override void OnUpdate(Double runningPct)
        {
            var currentValue = _initialValue + (_valueDifference * runningPct);
            var assigned = new AssignedStyle(_assignedStyle.SetterType, _assignedStyle.Type,
                currentValue);

            _updater(assigned);
        }
    }
}
