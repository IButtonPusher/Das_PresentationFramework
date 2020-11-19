using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.Views.Input
{
    public readonly struct MouseOverEventArgs : IMouseInputEventArgs
    {
        public MouseOverEventArgs(IPoint2D position,
                                  Boolean isMouseOver,
                                  IInputContext inputContext)
        {
            Position = position;
            InputContext = inputContext;
            IsMouseOver = isMouseOver;
        }

        public readonly Boolean IsMouseOver;

        public IPoint2D Position { get; }

        public InputAction Action => InputAction.MouseOver;

        public IInputContext InputContext { get; }
    }
}