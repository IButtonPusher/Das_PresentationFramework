using System;
using System.Threading.Tasks;

namespace Das.Views.Input
{
    public interface IHandleInput<in T> : IHandleInput//IInteractiveVisual
        where T : IInputEventArgs
    {
        Boolean OnInput(T args);
    }

    public interface IHandleInput //: IStatefulVisual
    {
        InputAction HandlesActions { get; }
    }
}