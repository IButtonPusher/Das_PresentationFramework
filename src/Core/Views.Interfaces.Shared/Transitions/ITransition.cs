using System;

namespace Das.Views.Transitions;

public interface ITransition<in T> : ITransition
{
   void SetValue(T startValue,
                 T endValue);
}

public interface ITransition
{
   TimeSpan Duration { get; }

   TimeSpan Delay { get; }

   TransitionFunctionType Timing { get; }
}