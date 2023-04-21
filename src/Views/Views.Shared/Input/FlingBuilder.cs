using System;
using System.Threading.Tasks;

namespace Das.Views.Input
{
   public class FlingBuilder
   {
      public FlingBuilder(Double dpiRatio,
                          Single scrollFriction,
                          Single physicalCoefficient)
      {
         _dpiRatio = dpiRatio;
         _scrollFriction = scrollFriction;
         _physicalCoefficient = physicalCoefficient;
      }

      public void BuildFlingValues(Double velocity,
                                    out Double distance,
                                    out TimeSpan duration)
      {
         if (velocity != 0)
         {
            distance = GetSplineFlingDistance(velocity * _dpiRatio);
            duration = GetSplineFlingDuration(velocity);
         }
         else
         {
            distance = 0;
            duration = TimeSpan.Zero;
         }
      }

      private Double GetSplineFlingDistance(Double velocity)
      {
         var l = GetSplineDeceleration(velocity);
         var decelMinusOne = _decelerationRate - 1.0;
         return _scrollFriction * _physicalCoefficient *
                Math.Exp(_decelerationRate / decelMinusOne * l) *
                Math.Sign(velocity);
      }

      private TimeSpan GetSplineFlingDuration(Double velocity)
      {
         var l = GetSplineDeceleration(velocity);
         var decelMinusOne = _decelerationRate - 1.0;
         return TimeSpan.FromMilliseconds(1000.0 * Math.Exp(l / decelMinusOne));
      }

      private Double GetSplineDeceleration(Double velocity) =>
         Math.Log(_inflexion * Math.Abs(velocity) /
                  (_scrollFriction * _physicalCoefficient));

      private static readonly Single _decelerationRate = (Single)(Math.Log(0.78) / Math.Log(0.9));
      private static readonly Single _inflexion = 0.35f; // Tension lines cross at (INFLEXION, 1)

      private readonly Double _dpiRatio;
      private readonly Single _physicalCoefficient;
      private readonly Single _scrollFriction;
   }
}
