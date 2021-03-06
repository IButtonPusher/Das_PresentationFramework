﻿using System;
using System.Threading.Tasks;

namespace Das.Views.Core.Drawing
{
    public class HatchBrush : IBrush, IEquatable<HatchBrush>
    {
        public HatchBrush(IColor backgroundColor,
                          HatchStyle hatchStyle)
            : this(backgroundColor,
                Color.FromRgb((Byte) (255 - backgroundColor.R),
                    (Byte) (255 - backgroundColor.G),
                    (Byte) (255 - backgroundColor.B)),
                hatchStyle)
        {
        }

        public HatchBrush(IColor backgroundColor,
                          IColor foregroundColor,
                          HatchStyle hatchStyle)
        {
            BackgroundColor = backgroundColor;
            ForegroundColor = foregroundColor;
            HatchStyle = hatchStyle;

            _hash = backgroundColor.GetHashCode() | foregroundColor.GetHashCode() |
                    (Int32) hatchStyle;
        }

        public Boolean Equals(IBrush other)
        {
            return other is HatchBrush hb && Equals(hb);
        }

        public Boolean Equals(HatchBrush other)
        {
            return other != null &&
                   other.ForegroundColor == ForegroundColor &&
                   other.BackgroundColor == BackgroundColor &&
                   other.HatchStyle == HatchStyle;
        }

        public IColor BackgroundColor { get; }

        public IColor ForegroundColor { get; }

        public HatchStyle HatchStyle { get; }

        public override Boolean Equals(Object obj)
        {
            return obj is HatchBrush hb && Equals(hb);
        }

        public override Int32 GetHashCode()
        {
            return _hash;
        }

        //private System.Drawing.Drawing2D.HatchBrush _gdi;
        private readonly Int32 _hash;

        //public static explicit operator System.Drawing.Brush(HatchBrush b)
        //{
        //    return b._gdi;
        //}
        public Boolean IsInvisible => false;

        public Double Opacity { get; set; } = 1;

        public IBrush GetWithOpacity(Double opacity)
        {
            return new HatchBrush(BackgroundColor, ForegroundColor, HatchStyle) {Opacity = opacity};
        }
    }
}