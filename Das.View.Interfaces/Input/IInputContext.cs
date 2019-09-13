using System;

namespace Das.Views.Input
{
    public interface IInputContext : IInputProvider
    {
        Boolean IsMousePresent { get; }
    }
}