using System;
using System.Collections.Generic;
using System.Text;

namespace Das.Views.Input
{
    public interface IInputEventArgs
    {
        IInputContext InputContext { get; }
    }
}
