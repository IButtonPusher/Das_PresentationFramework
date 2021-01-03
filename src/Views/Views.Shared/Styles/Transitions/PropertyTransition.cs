using System;
using System.Threading;
using System.Threading.Tasks;
using Das.Views.Transitions;

#if !NET40
using TaskEx = System.Threading.Tasks.Task;
#endif

namespace Das.Views.Styles.Transitions
{
    public abstract class PropertyTransition<T> : BaseTransition,
                                                  ITransition<T>
    {
        protected PropertyTransition(IVisualElement visual,
                                     IDependencyProperty<T> property,
                                     TimeSpan duration,
                                     TimeSpan delay,
                                     TransitionFunctionType timing)
            : base(duration, delay)
        {
            _visual = visual;
            _property = property;
            Duration = duration;
            Delay = delay;
            Timing = timing;

            CurrentStartValue = property.GetValue(visual);
            CurrentEndValue = property.GetValue(visual);
        }

        public virtual void SetValue(T startValue,
                                     T endValue)
        {
            CurrentStartValue = startValue;
            CurrentEndValue = endValue;

            TaskEx.Run(() => RunUpdates(CancellationToken.None)).ConfigureAwait(false);
        }

        protected override void OnFinished(Boolean wasCancelled)
        {
            _property.SetValueNoTransitions(_visual, CurrentEndValue);
        }

        protected abstract T GetCurrentValue(Double runningPct);

        protected override void OnUpdate(Double runningPct)
        {
            var value = GetCurrentValue(runningPct);
            _property.SetValueNoTransitions(_visual, value);
        }

        public TimeSpan Duration { get; }

        public TimeSpan Delay { get; }

        public TransitionFunctionType Timing { get; }

        private readonly IDependencyProperty _property;
        protected readonly IVisualElement _visual;

        protected T CurrentEndValue;

        protected T CurrentStartValue;
    }
}