﻿using System;

namespace Das.Views.Core.Geometry
{
    public class Thickness : Size, IDeepCopyable<Thickness>, IShape2d
    {
        public new static Thickness Empty { get; } = new Thickness(0);

        public Thickness(double uniformLength)
        {
            Left = Right = Top = Bottom = uniformLength;
        }

        public Thickness(double left, double top, double right, double bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public double Left { get; }

        public double Top { get; }

        public double Right { get; }

        public double Bottom { get; }

        public override Double Width => Left + Right;

        public override Double Height => Top + Bottom;

        public override Boolean IsEmpty => Height.AreEqualEnough(0) && Width.AreEqualEnough(0);

        public static Thickness operator +(Thickness size, Thickness margin)
        {
            if (margin == null)
                return size.DeepCopy();

            return new Thickness(size.Left + margin.Left, margin.Top + size.Top,
                margin.Right + size.Right, margin.Bottom + size.Bottom);
        }

        public static Thickness operator *(Thickness size, Double val)
        {
            if (val.AreEqualEnough(1))
                return size;

            if (size == null)
                return null;

            return new Thickness(size.Left * val, size.Top * val, size.Right * val,
                size.Bottom * val);
        }

        public static explicit operator Thickness(Double uniform)
            => new Thickness(uniform);

        public static explicit operator Thickness(Int32 uniform)
            => new Thickness(uniform);

        public new Thickness DeepCopy() => new Thickness(Left, Top, Right, Bottom);

        public override string ToString() => "L: " + Left.ToString("0.0") +
                                             " T: " + Top.ToString("0.0") +
                                             " R: " + Right.ToString("0.0") +
                                             " B: " + Bottom.ToString("0.0");
    }
}