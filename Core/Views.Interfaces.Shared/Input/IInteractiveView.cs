using System;
using System.Threading.Tasks;
using Das.Views.Input;
using Das.Views.Styles;

namespace Das.Views
{
    public interface IInteractiveView : IStatefulVisual
    {
        InputAction HandlesActions { get; }
    }
}