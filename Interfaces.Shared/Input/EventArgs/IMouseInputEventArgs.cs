using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.Views.Input
{
    public interface IMouseInputEventArgs<T> : IMouseInputEventArgs
        where T : IMouseInputEventArgs<T>
    {
        T Offset(IPoint2D offset);

        T Offset(Double pct);
    }


    public interface IMouseInputEventArgs : IInputEventArgs
    {
        IPoint2D Position { get; }
    }
}