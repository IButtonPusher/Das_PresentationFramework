using System;
using Das.Views.Core.Geometry;

namespace Das.Views.Input
{
    public interface IMouseInputEventArgs : IInputEventArgs
    {
        IPoint2D Position { get; }
    }
}
