using System;
using System.Collections.Generic;
using System.Text;
using Das.Views.Core.Geometry;

namespace Das.Views.Input
{
    public readonly struct LostCaptureEventArgs : IInputEventArgs//IMouseInputEventArgs<LostCaptureEventArgs>
    {
        public LostCaptureEventArgs(IInputContext inputContext)
        {
            InputContext = inputContext;
        }

        public InputAction Action => InputAction.LostCapture;

        public IInputContext InputContext { get; }

        //public IPoint2D Position { get; }

        //public LostCaptureEventArgs Offset(IPoint2D offset)
        //{
        //    return new LostCaptureEventArgs(InputContext, Position.Offset(offset));
        //}

        //public LostFocusEventArgs Offset(Double pct)
        //{
        //    return TODO_IMPLEMENT_ME;
        //}
    }
}
