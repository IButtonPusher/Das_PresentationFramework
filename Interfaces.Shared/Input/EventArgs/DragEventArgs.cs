using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.Views.Input
{
    public readonly struct DragEventArgs : IMouseInputEventArgs<DragEventArgs>
    {
        public DragEventArgs(IPoint2D startPosition,
                             IPoint2D currentPosition,
                             ISize lastChange,
                             MouseButtons button,
                             IInputContext inputContext)
        {
            StartPosition = startPosition;
            Position = currentPosition;
            LastChange = lastChange;
            Button = button;
            InputContext = inputContext;

            TotalDragged = currentPosition.Offset(startPosition);
        }

        public readonly IPoint2D StartPosition;

        public readonly ISize LastChange;

        public readonly IPoint2D TotalDragged;

        public readonly MouseButtons Button;

        public DragEventArgs Offset(IPoint2D offset)
        {
            return new DragEventArgs(StartPosition.Offset(offset),
                Position.Offset(offset), LastChange, Button, InputContext);
        }

        public DragEventArgs Offset(Double pct)
        {
            return new DragEventArgs(StartPosition.Offset(pct),
                Position.Offset(pct), 
                LastChange.Divide(pct), Button, InputContext);
        }

        public InputAction Action => InputAction.MouseDrag;

        public IInputContext InputContext { get; }

        public IPoint2D Position { get; }
    }
}