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
        private readonly IVisualElement _runningOnVisual;
        //private readonly Object? _initialValue;
        private readonly Transition _transition;
        private readonly IStyle _style;
        private readonly AssignedStyle _assignedStyle;
        private readonly Action<AssignedStyle> _updater;

        private readonly Double _initialValue;
        private readonly Double _finalValue;
        private readonly Double _valueDifference;

        public DoubleTransition(IVisualElement runningOnVisual,
                                 Object? initialValue,
                                 Transition transition,
                                 IStyle style,
                                 AssignedStyle assignedStyle,
                                 Action<AssignedStyle> updater)
            : base(Easing.QuadraticOut, transition.Duration, transition.Delay)
        {
            _runningOnVisual = runningOnVisual;
            _initialValue = initialValue is Double d ? d : 0;
            if (assignedStyle.Value is Double valid)
                _finalValue = valid;
            else 
                throw new NotImplementedException();

            _valueDifference = _finalValue - _initialValue;

            _transition = transition;
            _style = style;
            _assignedStyle = assignedStyle;
            _updater = updater;
        }

        public void Start()
        {
            TaskEx.Run(() => RunUpdates(CancellationToken.None)).ConfigureAwait(false);
        }

        //private async Task RunUpdates()
        //{
        //    await TaskEx.Delay(_transition.Delay);

        //    var running = Stopwatch.StartNew();
        //    var runningPct = 0.0;

        //    while (runningPct < 1)
        //    {
        //        runningPct = EaseOutQuadratic(runningPct);

        //        var currentValue = _initialValue + (_valueDifference * runningPct);
        //        var assigned = new AssignedStyle(_assignedStyle.Setter, _assignedStyle.Selector,
        //            currentValue);

        //        _updater(assigned);
        //        //_style.Add(_assignedStyle.Setter, _assignedStyle.Selector, currentValue);
                
        //        await TaskEx.Delay(SIXTY_FPS);
        //        runningPct = running.ElapsedMilliseconds / _transition.Duration.TotalMilliseconds;
        //    } 
        //}

        private const Int32 SIXTY_FPS = 1000 / 60;

        protected override void OnUpdate(Double runningPct)
        {
            var currentValue = _initialValue + (_valueDifference * runningPct);
            var assigned = new AssignedStyle(_assignedStyle.SetterType, _assignedStyle.Type,
                currentValue);

            _updater(assigned);
        }
    }
}
