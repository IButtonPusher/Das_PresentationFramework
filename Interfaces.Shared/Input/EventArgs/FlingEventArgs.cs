﻿using System;
using Das.Views.Core.Geometry;

namespace Das.Views.Input
{
    public readonly struct FlingEventArgs : IMouseInputEventArgs<FlingEventArgs>
    {
        public FlingEventArgs(Double velocityX, 
                              Double velocityY, 
                              IPoint2D position, 
                              IInputContext inputContext)
        {
            VelocityX = velocityX;
            VelocityY = velocityY;
            Position = position;
            InputContext = inputContext;
        }

        public readonly Double VelocityX;

        public readonly Double VelocityY;

        public IPoint2D Position { get; }

        public FlingEventArgs Offset(IPoint2D offset)
        {
            return new FlingEventArgs(VelocityX, VelocityY, Position.Offset(offset), InputContext);
        }

        public InputAction Action => InputAction.Fling;

        public IInputContext InputContext { get; }

        

    }
}
