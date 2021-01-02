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

        //private readonly AssignedStyle _assignedStyle;
        //private readonly Action<AssignedStyle> _updater;

        //private readonly Thickness _initialValue;

        //private readonly Double _leftDiff;
        //private readonly Double _topDiff;
        //private readonly Double _rightDiff;
        //private readonly Double _bottomDiff;

        //public ThicknessTransition(Object? initialValue,
        //                           Transition transition,
        //                           AssignedStyle assignedStyle,
        //                           Action<AssignedStyle> updater)
        // : base(transition.Duration, transition.Delay)
        //{
        //    _initialValue = initialValue is Thickness d ? d : new Thickness(0);
        //    if (!(assignedStyle.Value is Thickness finalValue))
        //        throw new NotImplementedException();

        //    _leftDiff = finalValue.Left - _initialValue.Left;
        //    _topDiff = finalValue.Top - _initialValue.Top;
        //    _rightDiff = finalValue.Right - _initialValue.Right;
        //    _bottomDiff = finalValue.Bottom - _initialValue.Bottom;

        //    _assignedStyle = assignedStyle;
        //    _updater = updater;
        //}

        //public void Start()
        //{
        //    //TaskEx.Run(RunUpdates).ConfigureAwait(false);
        //    TaskEx.Run(() => RunUpdates(CancellationToken.None)).ConfigureAwait(false);
        //}

        protected override QuantifiedThickness GetCurrentValue(Double runningPct)
        {
            return CurrentStartValue.Transition(CurrentEndValue, runningPct);
        }

        //protected override void OnUpdate(Double runningPct)
        //{
        //    var currentLeft = _initialValue.Left + (_leftDiff * runningPct);
        //    var currentTop = _initialValue.Top + (_topDiff * runningPct);
        //    var currentRight = _initialValue.Right+ (_rightDiff * runningPct);
        //    var currentBottom = _initialValue.Bottom+ (_bottomDiff * runningPct);
        //    var currentValue = new Thickness(currentLeft, currentTop,
        //        currentRight, currentBottom);

        //    var assigned = new AssignedStyle(_assignedStyle.SetterType, _assignedStyle.Type,
        //        currentValue);

        //    _updater(assigned);
        //}

      
    }
}
