using System;
using System.Collections.Generic;
using System.Text;
using Das.Views.Core.Geometry;
using Das.Views.Core.Input;

namespace Das.Views.Input
{
    public readonly struct MouseDownEventArgs : IMouseInputEventArgs
    {
        public MouseDownEventArgs(IPoint2D position, 
                              MouseButtons? button, 
                              IInputContext inputContext)
        {
            Position = position;
            Button = button;
            InputContext = inputContext;
        }

        public IPoint2D Position { get; }
        
        public readonly MouseButtons? Button;

        public IInputContext InputContext { get; }
    }
}
