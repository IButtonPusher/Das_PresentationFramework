using System;
using System.Threading.Tasks;

namespace Das.Views.Input
{
    public interface IHandleInput<in T> : IInteractiveView
        where T : IInputEventArgs
    {
        Boolean OnInput(T args);
    }
}