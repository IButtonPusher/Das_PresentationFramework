#if !NET40
using TaskEx = System.Threading.Tasks.Task;
#endif

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;
using Das.Views.Styles.Transitions;

namespace Das.Views.Styles
{
    public class ThicknessTransition : BaseTransition
    {
        private readonly AssignedStyle _assignedStyle;
        private readonly Action<AssignedStyle> _updater;

        private readonly Thickness _initialValue;

        private readonly Double _leftDiff;
        private readonly Double _topDiff;
        private readonly Double _rightDiff;
        private readonly Double _bottomDiff;

        public ThicknessTransition(Object? initialValue,
                                   Transition transition,
                                   AssignedStyle assignedStyle,
                                   Action<AssignedStyle> updater)
         : base(Easing.QuadraticOut, transition.Duration, transition.Delay)
        {
            _initialValue = initialValue is Thickness d ? d : new Thickness(0);
            if (!(assignedStyle.Value is Thickness finalValue))
                throw new NotImplementedException();

            _leftDiff = finalValue.Left - _initialValue.Left;
            _topDiff = finalValue.Top - _initialValue.Top;
            _rightDiff = finalValue.Right - _initialValue.Right;
            _bottomDiff = finalValue.Bottom - _initialValue.Bottom;

            _assignedStyle = assignedStyle;
            _updater = updater;
        }

        public void Start()
        {
            //TaskEx.Run(RunUpdates).ConfigureAwait(false);
            TaskEx.Run(() => RunUpdates(CancellationToken.None)).ConfigureAwait(false);
        }

        //private async Task RunUpdates()
        //{
        //    await TaskEx.Delay(_transition.Delay);

        //    var running = Stopwatch.StartNew();
        //    Double runningPct;

        //    while (true)
        //    {
        //        await TaskEx.Delay(SIXTY_FPS);

        //        runningPct = Math.Min(
        //            running.ElapsedMilliseconds / _transition.Duration.TotalMilliseconds, 1);

        //        runningPct = EaseOutQuadratic(runningPct);

        //        var currentLeft = _initialValue.Left + (_leftDiff * runningPct);
        //        var currentTop = _initialValue.Top + (_topDiff * runningPct);
        //        var currentRight = _initialValue.Right+ (_rightDiff * runningPct);
        //        var currentBottom = _initialValue.Bottom+ (_bottomDiff * runningPct);
        //        var currentValue = new Thickness(currentLeft, currentTop,
        //            currentRight, currentBottom);

        //        var assigned = new AssignedStyle(_assignedStyle.Setter, _assignedStyle.Selector,
        //            currentValue);

        //        _updater(assigned);
                

        //        if (runningPct >= 1)
        //            break;

        //    } 
        //}

        private const Int32 SIXTY_FPS = 1000 / 60;

        protected override void OnUpdate(Double runningPct)
        {
            var currentLeft = _initialValue.Left + (_leftDiff * runningPct);
            var currentTop = _initialValue.Top + (_topDiff * runningPct);
            var currentRight = _initialValue.Right+ (_rightDiff * runningPct);
            var currentBottom = _initialValue.Bottom+ (_bottomDiff * runningPct);
            var currentValue = new Thickness(currentLeft, currentTop,
                currentRight, currentBottom);

            var assigned = new AssignedStyle(_assignedStyle.Setter, _assignedStyle.Selector,
                currentValue);

            _updater(assigned);
        }
    }
}
