using System;
using Das.Views.Styles;

namespace Das.Views.Rendering
{
    public class Transition
    {
        public Transition(StyleSetterType setterType,
                          TimeSpan duration,
                          TimeSpan delay, 
                          TransitionTiming timing)
        {
            SetterType = setterType;
            Duration = duration;
            Delay = delay;
            Timing = timing;
        }

        public static readonly Transition[] EmptyTransitions = new Transition[0];

        public StyleSetterType SetterType { get; }

        public TimeSpan Duration { get; }

        public TimeSpan Delay { get; }

        public TransitionTiming Timing { get; }
    }
}
