using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Das.Extensions;

namespace Das.Views.Input;

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
                  _velocityX = args.DistanceFlungX;
                  break;

               case FlingMode.Inverted:
                  _velocityX = 0 - args.DistanceFlungX; //args.VelocityX;
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
                  _velocityY = args.DistanceFlungY;// args.VelocityY;
                  break;

               case FlingMode.Inverted:
                  _velocityY = 0 - args.DistanceFlungY;//args.VelocityY;
                  break;
               default:
                  throw new ArgumentOutOfRangeException();
            }

         if (_velocityX.IsZero() && _velocityY.IsZero())
         {
            return true;
         }

         if (_currentTransition is { } currentTransition)
         {
            var vx = currentTransition.VelocityX;
            var vy = currentTransition.VelocityY;

            Debug.WriteLine($"@@@@@@@@ existing fling: {vx},{vy} new fling: {args.VelocityX},{args.VelocityY}");

            currentTransition.Cancel();

         }

         //_currentTransition?.Cancel();

         var sumX = Convert.ToInt32(_velocityX);
         var sumY = Convert.ToInt32(_velocityY);

         var validVeticalRange = _flingHost.GetVerticalMinMaxFling();
         sumY = validVeticalRange.GetValueInRange(sumY);

         var validHorizontalRange = _flingHost.GetHorizontalMinMaxFling();
         sumX = validHorizontalRange.GetValueInRange(sumX);

         if (sumX.IsZero() && sumY.IsZero())
            return true;


         Debug.WriteLine("[OKYN] Created fling transition x,y: " + sumX + "," + sumY +
                         " duration: " + args.FlingXDuration + "," + args.FlingYDuration +
                         "\t\t\t\r\nbased on: " + args);

         var dur = args.FlingYDuration > args.FlingXDuration
            ? args.FlingYDuration
            : args.FlingXDuration;

         _currentTransition = new SplineFlingTransition(_flingHost,
            dur, sumX, sumY, TimeSpan.Zero);

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
   private SplineFlingTransition? _currentTransition;
   private Double _velocityX;
   private Double _velocityY;

   //private const Int32 _maxFlingMs = 3000;
}