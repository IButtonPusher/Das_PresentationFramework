using System;
// ReSharper disable UnusedMember.Global

namespace Das.Views.Transitions;

public enum TransitionFunctionType
{
   Invalid,
        
   Ease,
   Linear,
   EaseIn,
   EaseOut,
   EaseInOut,
   StepStart,
   StepEnd,
        
   Initial,
   Inherit
}