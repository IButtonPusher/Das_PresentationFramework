using System;

namespace Das.Views.Input
{
    public interface IInputVisual<out TVisual> : IInputVisual
        where TVisual : Enum
    {
        TVisual InputType { get; }
    }

    public interface IInputVisual
    {
        
    }
}
