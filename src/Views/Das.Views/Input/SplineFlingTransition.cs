using System;
using System.Threading;
using System.Threading.Tasks;
using Das.Views.Styles.Transitions;
using Das.Views.Transitions;

namespace Das.Views.Input;

/// <summary>
///    https://android.googlesource.com/platform/frameworks/base/+/refs/heads/android10-c2f2-release/core/java/android/widget/OverScroller.java
/// </summary>
public class SplineFlingTransition : BaseTransition, IManualTransition
{
   static SplineFlingTransition()
   {
      var x_min = 0.0f;
      var y_min = 0.0f;
      for (var i = 0; i < 100; i++)
      {
         Single x;
         Single coef;
         Single y;
         var alpha = i / 100.0f;
         var x_max = END_TENSION;
         while (true)
         {
            x = x_min + (x_max - x_min) / 2.0f;
            coef = 3.0f * x * (END_TENSION - x);
            var tx = ((END_TENSION - x) * P1 + P2 * x) * coef + x * x * x;
            if (Math.Abs(tx - alpha) < 1.0E-5d)
               break;
            if (tx > alpha)
               x_max = x;
            else
               x_min = x;
         }

         SPLINE_POSITION[i] = ((END_TENSION - x) * 0.5f + x) * coef + x * x * x;
         var y_max = END_TENSION;
         while (true)
         {
            y = y_min + (y_max - y_min) / 2.0f;
            coef = 3.0f * y * (END_TENSION - y);
            var dy = ((END_TENSION - y) * 0.5f + y) * coef + y * y * y;
            if (Math.Abs(dy - alpha) < 1.0E-5d)
               break;
            if (dy > alpha)
               y_max = y;
            else
               y_min = y;
         }

         SPLINE_TIME[i] = ((END_TENSION - y) * P1 + P2 * y) * coef + y * y * y;
      }

      var fArr = SPLINE_POSITION;
      SPLINE_TIME[100] = END_TENSION;
      fArr[100] = END_TENSION;
   }

   //public SplineFlingTransition(Double initialVelocity,
   //                             Single friction,
   //                             Single physicalCoefficient,
   //                             IFlingHost host,
   //                             TimeSpan delay) :
   //   this(host,
   //      GetSplineFlingDuration(initialVelocity, friction, physicalCoefficient),
   //      GetSplineFlingDistance(initialVelocity, friction, physicalCoefficient),
   //      delay)
   //{
   //}

   public SplineFlingTransition(IFlingHost host,
                                TimeSpan duration,
                                Double distanceX,
                                Double distanceY,
                                TimeSpan delay)
      : base(duration, delay, Easing.Linear)
   {
      _host = host;
      _distanceX = distanceX;
      _distanceY = distanceY;
      _cancellationSource = new CancellationTokenSource();

      _startX = host.CurrentX;
      _startY = host.CurrentY;
   }

   public void Cancel()
   {
      _cancellationSource.Cancel(false);
   }

   public override void Start()
   {
      Start(_cancellationSource.Token);
   }

   protected override void OnUpdate(Double runningPct)
   {
      var index = (Int32) (NB_SAMPLES * runningPct);
      var distanceCoef = 1.0f;
      if (index < NB_SAMPLES)
      {
         var t_inf = (Single) index / NB_SAMPLES;
         var t_sup = (Single) (index + 1) / NB_SAMPLES;
         var d_inf = SPLINE_POSITION[index];
         var d_sup = SPLINE_POSITION[index + 1];
         var velocityCoef = (d_sup - d_inf) / (t_sup - t_inf);
         distanceCoef = (Single) (d_inf + (runningPct - t_inf) * velocityCoef);
      }

      var distanceX = distanceCoef * _distanceX;
      var flungX = _host.CurrentX - _startX;
      var currentX = distanceX - flungX;


      var distanceY = distanceCoef * _distanceY;
      var flungY = _host.CurrentY - _startY;
      var currentY = distanceY - flungY;

      //Debug.WriteLine("[OKYN] Update splinef y: " + currentY.ToString("0.00") + 
      //                " pct: " + runningPct.ToString("0.00") + 
      //                " total: " + flungY);

      _host.OnFlingStep(currentX, currentY);

      //UpdateCurrentVelocity(velocityCoef * _distance / _splineDuration * 1000.0f,
      //    "update where index = " + index + " t = " + t);
   }

   protected override void OnFinished(Boolean wasCancelled)
   {
      base.OnFinished(wasCancelled);
      _host.OnFlingEnded(wasCancelled);
         
   }

   //private static Double GetSplineDeceleration(Double velocity,
   //                                            Single mFlingFriction,
   //                                            Single mPhysicalCoeff)
   //{
   //   return Math.Log(_inflexion * Math.Abs(velocity) / (mFlingFriction * mPhysicalCoeff));
   //}

   //private static Double GetSplineFlingDistance(Double velocity,
   //                                             Single mFlingFriction,
   //                                             Single mPhysicalCoeff)
   //{
   //   var l = GetSplineDeceleration(velocity, mFlingFriction, mPhysicalCoeff);
   //   var decelMinusOne = _decelerationRate - 1.0;
   //   return mFlingFriction * mPhysicalCoeff * Math.Exp(_decelerationRate / decelMinusOne * l);
   //}

   //private static TimeSpan GetSplineFlingDuration(Double velocity,
   //                                               Single mFlingFriction,
   //                                               Single mPhysicalCoeff)
   //{
   //   var l = GetSplineDeceleration(velocity, mFlingFriction, mPhysicalCoeff);
   //   var decelMinusOne = _decelerationRate - 1.0;
   //   return TimeSpan.FromMilliseconds(1000.0 * Math.Exp(l / decelMinusOne));
   //}

   private const Single START_TENSION = 0.5f;
   private const Single END_TENSION = 1.0f;

   private const Int32 NB_SAMPLES = 100;

   private static readonly Single INFLEXION = 0.35f; // Tension lines cross at (INFLEXION, 1)

   private static readonly Single P1 = START_TENSION * INFLEXION;
   private static readonly Single P2 = 1.0f - END_TENSION * (1.0f - INFLEXION);
   private static readonly Single[] SPLINE_POSITION = new Single[NB_SAMPLES + 1];
   private static readonly Single[] SPLINE_TIME = new Single[NB_SAMPLES + 1];

   //private static readonly Single _decelerationRate = (Single) (Math.Log(0.78) / Math.Log(0.9));
   //private static readonly Single _inflexion = 0.35f; // Tension lines cross at (INFLEXION, 1)

   private readonly CancellationTokenSource _cancellationSource;
   
   private readonly Double _distanceX;
   private readonly Double _distanceY;
   
   private readonly IFlingHost _host;

   private readonly Double _startX;
   private readonly Double _startY;
}