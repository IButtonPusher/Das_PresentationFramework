using System;
using Das.Views.Core.Geometry;

namespace Das.Views.Input
{
    public readonly struct DragEventArgs : IMouseInputEventArgs
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
        }

        public readonly IPoint2D StartPosition;

        public readonly ISize LastChange;
        
        public readonly MouseButtons Button;

        public IInputContext InputContext { get; }

        public IPoint2D Position { get; }
    }
}
