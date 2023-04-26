using System;

namespace Das.Views.Transitions;

public interface IManualTransition
{
   void Cancel();

   void Start();
}