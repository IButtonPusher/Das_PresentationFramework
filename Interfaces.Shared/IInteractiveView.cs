using System;
using System.Threading.Tasks;
using Das.Views.Input;

namespace Das.Views
{
    public interface IInteractiveView
    {
        InputAction HandlesActions { get; }
    }
}