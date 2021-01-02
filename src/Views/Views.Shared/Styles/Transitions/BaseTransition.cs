using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
#if !NET40
using TaskEx = System.Threading.Tasks.Task;

#endif

namespace Das.Views.Styles.Transitions
{
    public abstract class BaseTransition
    {
        public BaseTransition(TimeSpan duration,
                              TimeSpan delay)
        {
            _duration = duration;
            _delay = delay;
            
        }

        public virtual void Start(CancellationToken cancel)
        {
            Task.Factory.StartNew(() => RunUpdates(cancel)).ConfigureAwait(false);
        }

        protected static Double EaseOutQuadratic(Double pctComplete)
        {
            return 1 - (1 - pctComplete) * (1 - pctComplete);
        }


        protected static Double GetEaseOut(Double pctComplete)
        {
            return 1 - Math.Pow(1.0 - pctComplete, 5);
        }


        //protected static Double GetNextValue(Double pctComplete,
        //                                     Easing easing)
        //{
        //    switch (easing)
        //    {
        //        case Easing.QuadraticOut:
        //            return GetEaseOut(pctComplete);

        //        default:
        //            throw new ArgumentOutOfRangeException(nameof(easing), easing, null);
        //    }
        //}

        protected virtual void OnFinished(Boolean wasCancelled)
        {
        }

        protected abstract void OnUpdate(Double runningPct);

        protected virtual async Task RunUpdates(CancellationToken cancel)
        {
            await TaskEx.Delay(_delay).ConfigureAwait(false);
            if (cancel.IsCancellationRequested)
                return;

            var running = Stopwatch.StartNew();

            while (!cancel.IsCancellationRequested)
            {
                await TaskEx.Delay(SIXTY_FPS).ConfigureAwait(false);

                var runningPct = Math.Min(
                    running.ElapsedMilliseconds / _duration.TotalMilliseconds, 1);

                runningPct = EaseOutQuadratic(runningPct);

                OnUpdate(runningPct);

                if (runningPct >= 1)
                    break;
            }

            OnFinished(!cancel.IsCancellationRequested);
        }

        private const Int32 SIXTY_FPS = 1000 / 60;
        private readonly TimeSpan _delay;
        private readonly TimeSpan _duration;
    }
}