using System;
using System.Collections.Generic;
using System.Text;

namespace Das.Views.Input
{
    public interface IHandleInput<in T> : IInteractiveView
        where T : IInputEventArgs
    {
        Boolean OnInput(T args);
    }
}
