using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
#if !NET40
using TaskEx = System.Threading.Tasks.Task;

#endif

namespace Das.Views.Styles.Transitions;

public abstract class BaseTransition
{
   protected BaseTransition(TimeSpan duration,
                            TimeSpan delay,
                            Easing easing)
   {
      _duration = duration;
      _delay = delay;
      _easing = easing;

      TransitionState = TransitionState.PendingStart;
   }

   public TransitionState TransitionState { get; protected set; }

   public virtual void Start()
   {
      Start(new CancellationTokenSource().Token);
   }

   public virtual void Start(CancellationToken cancel)
   {
      Task.Factory.StartNew(() => RunUpdates(cancel)).ConfigureAwait(false);
   }

   protected static Double EaseOutQuadratic(Double pctComplete)
   {
      return 1 - (1 - pctComplete) * (1 - pctComplete);
   }

   protected static Double EaseOutQuint(Double pctComplete)
   {
      return 1 - Math.Pow(1.0 - pctComplete, 5);
   }

   protected virtual void OnFinished(Boolean wasCancelled)
   {
   }

   protected abstract void OnUpdate(Double runningPct);

   protected virtual async Task RunUpdates(CancellationToken cancel)
   {
      if (_delay.TotalMilliseconds > 0)
         await TaskEx.Delay(_delay).ConfigureAwait(false);

      if (cancel.IsCancellationRequested)
      {
         TransitionState = TransitionState.Cancelled;
         return;
      }

      var running = Stopwatch.StartNew();
      var swUpdate = new Stopwatch();

      while (!cancel.IsCancellationRequested)
      {
         //await TaskEx.Delay(SIXTY_FPS).ConfigureAwait(false);

         TransitionState = TransitionState.Running;

         var runningPct = Math.Min(
            running.ElapsedMilliseconds / _duration.TotalMilliseconds, 1);
   
         switch (_easing)
         {
            case Easing.QuadraticOut:
               runningPct = EaseOutQuadratic(runningPct);
               break;

            case Easing.QuintOut:
               runningPct = EaseOutQuint(runningPct);
               break;

            case Easing.Linear:
               //intentionally blank
               break;

            default:
               throw new ArgumentOutOfRangeException();
         }

         swUpdate.Restart();

         OnUpdate(runningPct);

         var elapsedMs = swUpdate.ElapsedMilliseconds;

         if (runningPct >= 1)
         {
            TransitionState = TransitionState.Finished;
            break;
         }

         var waitFor = SIXTY_FPS - elapsedMs;
         if (waitFor > 0 && waitFor < Int32.MaxValue)
         {
            await TaskEx.Delay((Int32)waitFor).ConfigureAwait(false);
         }
      }

      OnFinished(cancel.IsCancellationRequested);
   }

   private const Int32 SIXTY_FPS = 1000 / 60;
   private readonly TimeSpan _delay;
   private readonly Easing _easing;

   protected readonly TimeSpan _duration;
   //private readonly Func<Double, Double> _getCurrentPercent;
}
