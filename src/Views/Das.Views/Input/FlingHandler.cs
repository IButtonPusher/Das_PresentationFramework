using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Views.Transitions;

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


                if (args.VelocityY != 0)
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

                if (_velocityX.IsZero() && _velocityY.IsZero())
                {
                    args.SetHandled(true);
                    return true;
                }

                _currentTransition?.Cancel();

                //var splineDist = GetSplineFlingDistance(Convert.ToInt32(args.VelocityY),
                //    args.EffectiveFriction, args.PhysicalCoefficient);
                //var splineDist2 = GetSplineFlingDistance(Convert.ToInt32(args.VelocityY / args.InputContext.ZoomLevel),
                //    args.EffectiveFriction, args.PhysicalCoefficient);

                //var mSplineDistance = (int) (splineDist * Math.Sign(args.VelocityY));


                //var splineDur = GetSplineFlingDuration(Convert.ToInt32(args.VelocityY),
                //    args.EffectiveFriction, args.PhysicalCoefficient);

                //var splineDur2 = GetSplineFlingDuration(Convert.ToInt32(args.VelocityY/ args.InputContext.ZoomLevel),
                //        args.EffectiveFriction, args.PhysicalCoefficient);

                    

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

                Debug.WriteLine("[OKYN] Created fling transition x,y: " + sumX +
                                "," + sumY + " duration: " + duration +
                                " based on: " + args);

                for (var c = 1; c <= 10; c++)
                {
                    var decel = c * 500;
                    var splineDist = SplineFlingTransition.GetSplineDistance(args.VelocityY, decel);
                    var splineDur = SplineFlingTransition.GetSplineDuration(args.VelocityY, decel);

                    Debug.WriteLine("[OKYN] DECELERATION: " + decel +
                                    "\r\n------------------------\r\n Spline-y would have been dist: " +
                                    splineDist.ToString("0.00") +
                                    " duration: " + splineDur + "\r\n");
                    //+ " or dist: " + splineDist +
                    //" duration: " + splineDur2);
                }

                

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

        private Double GetSplineDeceleration(Int32 velocity,
                                             Single mFlingFriction,
                                             Single mPhysicalCoeff)
        {
            return Math.Log(_inflexion * Math.Abs(velocity) / (mFlingFriction * mPhysicalCoeff));
        }

        private Double GetSplineFlingDistance(Int32 velocity,
                                              Single mFlingFriction,
                                              Single mPhysicalCoeff)
        {
            var l = GetSplineDeceleration(velocity, mFlingFriction, mPhysicalCoeff);
            var decelMinusOne = _decelerationRate - 1.0;
            return mFlingFriction * mPhysicalCoeff * Math.Exp(_decelerationRate / decelMinusOne * l);
        }

        private Int32 GetSplineFlingDuration(Int32 velocity,
                                             Single mFlingFriction,
                                             Single mPhysicalCoeff)
        {
            var l = GetSplineDeceleration(velocity, mFlingFriction, mPhysicalCoeff);
            var decelMinusOne = _decelerationRate - 1.0;
            return (Int32) (1000.0 * Math.Exp(l / decelMinusOne));
        }

        private static readonly Single _decelerationRate = (Single) (Math.Log(0.78) / Math.Log(0.9));
        private static readonly Single _inflexion = 0.35f; // Tension lines cross at (INFLEXION, 1)

        private readonly IFlingHost _flingHost;
        private readonly Object _flingLock;
        private IManualTransition? _currentTransition;
        private Double _velocityX;
        private Double _velocityY;

        //private const Int32 _maxFlingMs = 3000;
    }
}
