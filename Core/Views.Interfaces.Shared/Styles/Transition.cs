using System;
using Das.Views.Styles;

namespace Das.Views.Rendering
{
    public class Transition
    {
        public Transition(StyleSetter setter,
                          TimeSpan duration,
                          TimeSpan delay, 
                          TransitionTiming timing)
        {
            Setter = setter;
            Duration = duration;
            Delay = delay;
            Timing = timing;
        }

        public static readonly Transition[] EmptyTransitions = new Transition[0];

        public StyleSetter Setter { get; }

        public TimeSpan Duration { get; }

        public TimeSpan Delay { get; }

        public TransitionTiming Timing { get; }
    }
}
