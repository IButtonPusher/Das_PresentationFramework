using System;
using System.Diagnostics;
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

            // 'slide' for 3 seconds max 
            var deceleration = args.InputContext.MaximumFlingVelocity / 3;

            

            lock (_flingLock)
            {
                if (_flingHost.CanFlingHorizontal && args.VelocityX != 0)
                    _velocityX = 0 - args.VelocityX;

                if (_flingHost.CanFlingVertical && args.VelocityY != 0)
                    _velocityY = args.VelocityY;

                if (_velocityX.IsZero() && _velocityY.IsZero())
                    return true;

                ////////////////////////////////////////////////

                _currentTransition?.Cancel();

                var sumX = Convert.ToInt32(_velocityX / 3);
                var sumY = Convert.ToInt32(_velocityY / 3);

                var validVeticalRange = _flingHost.GetVerticalMinMaxFling();
                sumY = validVeticalRange.GetValueInRange(sumY);

                var validHorizontalRange = _flingHost.GetHorizontalMinMaxFling();
                sumX = validHorizontalRange .GetValueInRange(sumX);

                var duration = TimeSpan.FromMilliseconds(
                    Math.Max(
                        Math.Abs(sumX), 
                        Math.Abs(sumY)));

                _currentTransition = new FlingTransition(duration, sumX, sumY, _flingHost);
                _currentTransition.Start();

                //dothNotify = true;

                //if (dothNotify)
                    _flingHost.OnFlingStarting(sumX, sumY);

                ////////////////////////////////////////////////

                //if (_flingProcessCounter == 0)
                //{
                //    dothNotify = true;

                //    _flingProcessCounter++;
                //    Task.Factory.StartNew(() => ProcessPendingFlings(deceleration)).ConfigureAwait(false);
                //}

                //if (dothNotify)
                //    _flingHost.OnFlingStarting(_velocityX, _velocityY);

                ////////////////////////////////////////////////
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

            //if (_velocityX == 0 && _velocityY == 0)
            //    return false;
            //_velocityX = _velocityY = 0;
            //return true;
        }

        //private async Task ProcessPendingFlings(Double deceleration)
        //{
        //    var swCurrent = Stopwatch.StartNew();

        //    var stepCoefficient = 100.0;
        //    var decelerateBy = deceleration / stepCoefficient;

        //    while (true)
        //    {
        //        //Debug.WriteLine("Velocities are: " + _velocityX + ", " + _velocityY);

        //        Double stepX;
        //        Double stepY;
        //        lock (_flingLock)
        //        {
        //            if (_velocityX > 0)
        //            {
        //                _velocityX = Math.Max(_velocityX - decelerateBy, 0);
        //                stepX = _velocityX / stepCoefficient;
        //            }
        //            else if (_velocityX < 0)
        //            {
        //                _velocityX = Math.Min(_velocityX + decelerateBy, 0);
        //                stepX = _velocityX / stepCoefficient;
        //            }
        //            else
        //                stepX = 0;

        //            //////////////////////////

        //            if (_velocityY > 0)
        //            {
        //                _velocityY = Math.Max(_velocityY - decelerateBy, 0);
        //                stepY = _velocityY / stepCoefficient;
        //            }
        //            else if (_velocityY < 0)
        //            {
        //                _velocityY = Math.Min(_velocityY + decelerateBy, 0);
        //                stepY = _velocityY / stepCoefficient;
        //            }
        //            else
        //                stepY = 0;

        //            if (stepY == 0 && stepX == 0)
        //            {
        //                _flingProcessCounter--;
        //                _flingHost.OnFlingStarting(stepX, stepY);
        //                return;
        //            }
        //        }

        //        //Debug.WriteLine(System.Threading.Thread.CurrentThread.ManagedThreadId + ": on fling stepping");
        //        //_onFlingStep(stepX, stepY);
        //        _flingHost.OnFlingStep(stepX, stepY);

        //        await Task.Delay(10);
        //        //Thread.Sleep(10);

        //        stepCoefficient = 1000.0 / swCurrent.ElapsedMilliseconds;
        //        decelerateBy = deceleration / stepCoefficient;

        //        swCurrent.Restart();
        //    }
        //}

        private readonly IFlingHost _flingHost;

        //private readonly T _hostVisual;
        //private readonly Func<T, Boolean> _canFlingHorizontal;
        //private readonly Func<T, Boolean> _canFlingVertical;

        private readonly Object _flingLock;

        private FlingTransition? _currentTransition;

        //private readonly Action<Double, Double> _onFlingStep;
        private Int64 _flingProcessCounter;


        private Double _velocityX;
        private Double _velocityY;
    }
}