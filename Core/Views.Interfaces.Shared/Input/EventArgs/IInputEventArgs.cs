using System;
using System.Threading.Tasks;

namespace Das.Views.Input
{
    public interface IInputEventArgs
    {
        InputAction Action { get; }

        IInputContext InputContext { get; }
    }
}