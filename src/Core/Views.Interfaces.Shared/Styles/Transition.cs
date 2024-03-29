﻿using System;
using Das.Views.Styles;
using Das.Views.Transitions;

namespace Das.Views.Rendering;

public class Transition
{
   public Transition(StyleSetterType setterType,
                     TimeSpan duration,
                     TimeSpan delay, 
                     TransitionFunctionType timing)
   {
      SetterType = setterType;
      Duration = duration;
      Delay = delay;
      Timing = timing;
   }

   public static readonly Transition[] EmptyTransitions =
      #if NET40
           new Transition[0];
      #else
      Array.Empty<Transition>();
   #endif

   public StyleSetterType SetterType { get; }

   public TimeSpan Duration { get; }

   public TimeSpan Delay { get; }

   public TransitionFunctionType Timing { get; }
}