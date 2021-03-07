using System;
using Das.Views.Styles.Transitions;
using Das.Views.Transitions;

namespace Das.Views.Input
{
    public class SplineFlingTransition : BaseTransition, IManualTransition
    {
        public SplineFlingTransition(TimeSpan duration,
                 TimeSpan delay,
                 Easing easing) : base(duration, delay, easing)
        {
        }

        public static Double GetSplineDuration(Double initialVelocity,
                                               Double deceleration)
        {
            var finalVelocity = 0;
            var brakingTime = (finalVelocity - initialVelocity) / (deceleration * 3.6);
            return brakingTime;
        }

        public static Double GetSplineDistance(Double initialVelocity,
                                               Double deceleration)
        {
            var finalVelocity = 0;
            var brakingDistance = (finalVelocity * finalVelocity - initialVelocity * initialVelocity)
                                  / (2 * deceleration * 3.6 * 3.6);

            return brakingDistance;
        }

        protected override void OnUpdate(Double runningPct)
        {
            
        }

        public void Cancel()
        {
            
        }

        public void Start()
        {
            
        }
    }
}
