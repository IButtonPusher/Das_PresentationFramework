using System;
using System.Threading.Tasks;

namespace Das.Views.Input
{
    public interface IMouseButtonEventArgs<T> : IMouseButtonEventArgs,
                                                IMouseInputEventArgs<T>
        where T : IMouseButtonEventArgs<T>
    {
    }

    public interface IMouseButtonEventArgs : IMouseInputEventArgs
    {
        MouseButtons Button { get; }
    }
}