using System;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Views.Styles;

namespace Das.Views.Input
{
    public class FlingHandler : IHandleInput<FlingEventArgs>,
                                IHandleInput<MouseDownEventArgs>
    {
        public FlingHandler(IFlingHost flingHost)
        {
            _flingHost = flingHost;
            _flingLock = new Object();
        }

        public Boolean OnInput(FlingEventArgs args)
        {
            lock (_flingLock)
            {
                if (_flingHost.CanFlingHorizontal && args.VelocityX != 0)
                    _velocityX = 0 - args.VelocityX;

                if (_flingHost.CanFlingVertical && args.VelocityY != 0)
                    _velocityY = args.VelocityY;

                if (_velocityX.IsZero() && _velocityY.IsZero())
                    return true;


                _currentTransition?.Cancel();

                var sumX = 0 - Convert.ToInt32(_velocityX / 3);
                var sumY = Convert.ToInt32(_velocityY / 3);

                var validVeticalRange = _flingHost.GetVerticalMinMaxFling();
                sumY = validVeticalRange.GetValueInRange(sumY);

                var validHorizontalRange = _flingHost.GetHorizontalMinMaxFling();
                sumX = validHorizontalRange .GetValueInRange(sumX);

                if (sumX.IsZero() && sumY.IsZero())
                    return true;

                var ms = Math.Max(
                    Math.Abs(sumX),
                    Math.Abs(sumY));
                ms = Math.Max(ms, 500);

                var duration = TimeSpan.FromMilliseconds(ms);

                _currentTransition = new FlingTransition(duration, sumX, sumY, _flingHost);
                _currentTransition.Start();

                
                _flingHost.OnFlingStarting(sumX, sumY);

            }

            return true;
        }

        public InputAction HandlesActions => InputAction.Fling;

        public StyleSelector CurrentStyleSelector => StyleSelector.None;

        public Boolean OnInput(MouseDownEventArgs args)
        {
            lock (_flingLock)
            {
                if (_currentTransition == null)
                    return false;

                _currentTransition.Cancel();
                _currentTransition = null;
                return true;
            }

        }

        private readonly IFlingHost _flingHost;
        private readonly Object _flingLock;
        private FlingTransition? _currentTransition;
        private Double _velocityX;
        private Double _velocityY;
    }
}