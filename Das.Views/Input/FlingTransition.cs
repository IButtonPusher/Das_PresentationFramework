using System;
using System.Threading;
using System.Threading.Tasks;
using Das.Views.Styles.Transitions;

namespace Das.Views.Input
{
    public class FlingTransition : BaseTransition
    {
        public FlingTransition(TimeSpan duration,
                               Double flingX,
                               Double flingY,
                               IFlingHost host)
            : base(Easing.QuadraticOut, duration, TimeSpan.Zero)
        {
            _flingX = flingX;
            _flingY = flingY;
            _host = host;
            _cancellationSource = new CancellationTokenSource();
        }

        public void Cancel()
        {
            _cancellationSource.Cancel(false);
        }

        public void Start()
        {
            Start(_cancellationSource.Token);
        }

        protected override void OnUpdate(Double runningPct)
        {
            var currentX = _flingX * runningPct - _flungX;
            var currentY = _flingY * runningPct - _flungY;

            System.Diagnostics.Debug.WriteLine("[OKYN" + Thread.CurrentThread.ManagedThreadId + "] " +
                            "update fling: " + currentX + "," + currentY +
                            " total: " + _flungY);

            _host.OnFlingStep(currentX, currentY);

            _flungX += currentX;
            _flungY += currentY;
        }

        protected override void OnFinished(Boolean wasCancelled)
        {
            base.OnFinished(wasCancelled);
            _host.OnFlingEnded(wasCancelled);
        }

        private readonly CancellationTokenSource _cancellationSource;
        private readonly Double _flingX;
        private readonly Double _flingY;
        private readonly IFlingHost _host;

        private Double _flungX;
        private Double _flungY;
    }
}