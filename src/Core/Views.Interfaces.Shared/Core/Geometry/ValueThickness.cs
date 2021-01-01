﻿using System;
using Das.Extensions;

namespace Das.Views.Core.Geometry
{
    public readonly struct  ValueThickness : IThickness
    {
        public ValueThickness(Double left, 
                              Double top,
                              Double right, 
                              Double bottom)
        {
            Bottom = bottom;
            Left = left;
            Right = right;
            Top = top;

            IsEmpty = left.IsZero() &&
                      right.IsZero() &&
                      top.IsZero() &&
                      bottom.IsZero();
        }

        public override String ToString()
        {
            return $"T: {Top} R: {Right} B: {Bottom} L: {Left}";
        }

        public Boolean IsEmpty { get; }

        public Double Bottom { get; }

        public Double Left { get; }

        public Double Right { get; }

        public Double Width => Left + Right;

        public Double Height => Top + Bottom;

        public Double Top { get; }
    }
}