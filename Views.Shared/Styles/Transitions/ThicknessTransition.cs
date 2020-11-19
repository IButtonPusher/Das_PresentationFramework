#if !NET40
using TaskEx = System.Threading.Tasks.Task;
#endif

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;
using Das.Views.Styles.Transitions;

namespace Das.Views.Styles
{
    public class ThicknessTransition : BaseTransition
    {
        private readonly IVisualElement _runningOnVisual;
        //private readonly Object? _initialValue;
        private readonly Transition _transition;
        private readonly IStyle _style;
        private readonly AssignedStyle _assignedStyle;
        private readonly Action<AssignedStyle> _updater;

        private readonly Thickness _initialValue;
        private readonly Thickness _finalValue;

        private readonly Double _leftDiff;
        private readonly Double _topDiff;
        private readonly Double _rightDiff;
        private readonly Double _bottomDiff;

        public ThicknessTransition(IVisualElement runningOnVisual,
                                   Object? initialValue,
                                   Transition transition,
                                   IStyle style,
                                   AssignedStyle assignedStyle,
                                   Action<AssignedStyle> updater)
        {
            _runningOnVisual = runningOnVisual;
            _initialValue = initialValue is Thickness d ? d : new Thickness(0);
            if (assignedStyle.Value is Thickness valid)
                _finalValue = valid;
            else 
                throw new NotImplementedException();

            _leftDiff = _finalValue.Left - _initialValue.Left;
            _topDiff = _finalValue.Top - _initialValue.Top;
            _rightDiff = _finalValue.Right - _initialValue.Right;
            _bottomDiff = _finalValue.Bottom - _initialValue.Bottom;

            _transition = transition;
            _style = style;
            _assignedStyle = assignedStyle;
            _updater = updater;
        }

        public void Start()
        {
            TaskEx.Run(RunUpdates).ConfigureAwait(false);
        }

        private async Task RunUpdates()
        {
            await TaskEx.Delay(_transition.Delay);

            var running = Stopwatch.StartNew();
            Double runningPct;

            while (true)
            {
                await TaskEx.Delay(SIXTY_FPS);

                runningPct = Math.Min(
                    running.ElapsedMilliseconds / _transition.Duration.TotalMilliseconds, 1);

                runningPct = EaseOutQuadratic(runningPct);

                var currentLeft = _initialValue.Left + (_leftDiff * runningPct);
                var currentTop = _initialValue.Top + (_topDiff * runningPct);
                var currentRight = _initialValue.Right+ (_rightDiff * runningPct);
                var currentBottom = _initialValue.Bottom+ (_bottomDiff * runningPct);
                var currentValue = new Thickness(currentLeft, currentTop,
                    currentRight, currentBottom);

                var assigned = new AssignedStyle(_assignedStyle.Setter, _assignedStyle.Selector,
                    currentValue);

                //var currentValue = _initialValue + (_valueDifference * runningPct);
                _updater(assigned);
                //_style.Add(_assignedStyle.Setter, _assignedStyle.Selector, currentValue);

                if (runningPct >= 1)
                    break;

            } 
        }

        private const Int32 SIXTY_FPS = 1000 / 60;
    }
}
