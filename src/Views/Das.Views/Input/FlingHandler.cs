using System;
using System.Threading.Tasks;
using Das.Extensions;

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
                if (args.VelocityX != 0)
                {
                    switch (_flingHost.HorizontalFlingMode)
                    {
                        case FlingMode.None:
                            break;

                        case FlingMode.Default:
                            _velocityX = args.VelocityX;
                            break;

                        case FlingMode.Inverted:
                            _velocityX = 0 - args.VelocityX;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }


                if (args.VelocityY != 0)
                {
                    switch (_flingHost.VerticalFlingMode)
                    {
                        case FlingMode.None:
                            break;

                        case FlingMode.Default:
                            _velocityY = args.VelocityY;
                            break;

                        case FlingMode.Inverted:
                            _velocityY = 0 - args.VelocityY;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                if (_velocityX.IsZero() && _velocityY.IsZero())
                {
                    args.SetHandled(true);
                    return true;
                }

                _currentTransition?.Cancel();

                var sumX = Convert.ToInt32(_velocityX / 3);

                //var sumY = Convert.ToInt32(_velocityY / 3);
                var sumY = Convert.ToInt32(_velocityY / 2);

                var validVeticalRange = _flingHost.GetVerticalMinMaxFling();
                sumY = validVeticalRange.GetValueInRange(sumY);

                var validHorizontalRange = _flingHost.GetHorizontalMinMaxFling();
                sumX = validHorizontalRange.GetValueInRange(sumX);

                if (sumX.IsZero() && sumY.IsZero())
                    return true;

                var ms = Math.Max(
                    Math.Abs(sumX),
                    Math.Abs(sumY));
                ms = Math.Max(ms, 500);

                var duration = TimeSpan.FromMilliseconds(ms);

                System.Diagnostics.Debug.WriteLine("Created fling transition x,y: " + sumX +
                                                   "," + sumY + " duration: " + duration +
                                                   " based on: " + args);

                _currentTransition = new FlingTransition(duration, sumX, sumY, 
                    _flingHost, args);
                _currentTransition.Start();

                
                _flingHost.OnFlingStarting(sumX, sumY);

            }

            return true;
        }

       
        public InputAction HandlesActions => InputAction.Fling;

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

        //private const Int32 _maxFlingMs = 3000;
    }
}