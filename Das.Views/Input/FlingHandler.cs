using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Das.Views.Styles;

namespace Das.Views.Input
{
    public class FlingHandler : IHandleInput<FlingEventArgs>,
                                IHandleInput<MouseDownEventArgs>
    {
        public FlingHandler(Func<Boolean> canFlingHorizontal,
                            Func<Boolean> canFlingVertical,
                            Action<Double, Double> onFlingStep)
        {
            _flingLock = new Object();

            _canFlingHorizontal = canFlingHorizontal;
            _canFlingVertical = canFlingVertical;
            _onFlingStep = onFlingStep;
        }

        public Boolean OnInput(FlingEventArgs args)
        {
            if (!_canFlingHorizontal() && !_canFlingVertical())
                return false;

            // 'slide' for 3 seconds max 
            var deceleration = args.InputContext.MaximumFlingVelocity / 3;

            lock (_flingLock)
            {
                if (_canFlingHorizontal() && args.VelocityX != 0)
                    _velocityX = args.VelocityX;

                if (_canFlingVertical() && args.VelocityY != 0)
                    _velocityY = args.VelocityY;

                if (_flingProcessCounter == 0)
                {
                    _flingProcessCounter++;
                    Task.Factory.StartNew(() => ProcessPendingFlings(deceleration)).ConfigureAwait(false);
                }
            }

            return true;
        }

        public InputAction HandlesActions => InputAction.Fling;

        public StyleSelector CurrentStyleSelector => StyleSelector.None;

        public Boolean OnInput(MouseDownEventArgs args)
        {
            if (_velocityX == 0 && _velocityY == 0)
                return false;
            _velocityX = _velocityY = 0;
            return true;
        }

        private void ProcessPendingFlings(Double deceleration)
        {
            var swCurrent = Stopwatch.StartNew();

            var stepCoefficient = 100.0;
            var decelerateBy = deceleration / stepCoefficient;

            while (true)
            {
                
                //Debug.WriteLine("Velocities are: " + _velocityX + ", " + _velocityY);

                Double stepX;
                Double stepY;
                lock (_flingLock)
                {
                    if (_velocityX > 0)
                    {
                        _velocityX = Math.Max(_velocityX - decelerateBy, 0);
                        stepX = _velocityX / stepCoefficient;
                    }
                    else if (_velocityX < 0)
                    {
                        _velocityX = Math.Min(_velocityX + decelerateBy, 0);
                        stepX = _velocityX / stepCoefficient;
                    }
                    else
                    {
                        stepX = 0;
                    }

                    //////////////////////////

                    if (_velocityY > 0)
                    {
                        _velocityY = Math.Max(_velocityY - decelerateBy, 0);
                        stepY = _velocityY / stepCoefficient;
                    }
                    else if (_velocityY < 0)
                    {
                        _velocityY = Math.Min(_velocityY + decelerateBy, 0);
                        stepY = _velocityY / stepCoefficient;
                    }
                    else
                    {
                        stepY = 0;
                    }

                    if (stepY == 0 && stepX == 0)
                    {
                        _flingProcessCounter--;
                        return;
                    }
                }

                _onFlingStep(stepX, stepY);

                Thread.Sleep(10);

                stepCoefficient = 1000.0 / swCurrent.ElapsedMilliseconds;
                decelerateBy = deceleration / stepCoefficient;

                swCurrent.Restart();
            }
        }

        private readonly Func<Boolean> _canFlingHorizontal;
        private readonly Func<Boolean> _canFlingVertical;

        private readonly Object _flingLock;
        private readonly Action<Double, Double> _onFlingStep;
        private Int64 _flingProcessCounter;


        private Double _velocityX;
        private Double _velocityY;
    }
}