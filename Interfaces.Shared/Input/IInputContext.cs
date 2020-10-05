using System;
using System.Threading.Tasks;

namespace Das.Views.Input
{
    public interface IInputContext : IInputProvider
    {
        Boolean IsMousePresent { get; }
    }
}