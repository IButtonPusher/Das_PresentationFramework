using System;

namespace Das.Views.Styles.Transitions
{
    public class BaseTransition
    {

        protected static Double GetEaseOut(Double pctComplete)
        {
            return 1 - Math.Pow(1.0 - pctComplete, 5);
        }

        protected static Double EaseOutQuadratic(Double pctComplete)
        {
            return 1 - (1 - pctComplete) * (1 - pctComplete);
        }
    }
}
