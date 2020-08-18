using System;
using System.Collections.Generic;
using System.Text;
using Das.Views.Input;
using Das.Views.Panels;

namespace Das.Views
{
    public interface IInteractiveView 
    {
        InputAction HandlesActions { get; }
    }
}
