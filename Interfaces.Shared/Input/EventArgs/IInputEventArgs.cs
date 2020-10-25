using System;

namespace Das.Views.Input
{
    public interface IInputEventArgs
    {
        InputAction Action { get; }

        IInputContext InputContext { get; }
    }
}
