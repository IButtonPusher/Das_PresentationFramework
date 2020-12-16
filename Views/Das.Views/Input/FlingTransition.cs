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
            System.Diagnostics.Debug.WriteLine("Created fling transition x,y: " + flingX + 
                                               "," + flingY + " duration: " + duration + 
                                               " visual: " + host);
            
            _flingX = flingX;
            _flingY = flingY;

            _startX = host.CurrentX;
            _startY = host.CurrentY;
            
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
            var flungX = _host.CurrentX - _startX;
            var flungY = _host.CurrentY - _startY;
            
            var currentX = _flingX * runningPct - flungX;
            var currentY = _flingY * runningPct - flungY;

            _host.OnFlingStep(currentX, currentY);
        }

        protected override void OnFinished(Boolean wasCancelled)
        {
            base.OnFinished(wasCancelled);
            _host.OnFlingEnded(wasCancelled);
        }

        private readonly CancellationTokenSource _cancellationSource;

        private readonly Double _flingX;
        private readonly Double _flingY;
        
        private readonly Double _startX;
        private readonly Double _startY;
        
        private readonly IFlingHost _host;
    }
}