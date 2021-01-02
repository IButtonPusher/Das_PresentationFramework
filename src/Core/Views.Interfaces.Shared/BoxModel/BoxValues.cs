using System;
using Das.Views.Core;

namespace Das.Views.BoxModel
{
    public readonly struct BoxValues<T> : IBoxValue<T>
    {
        public BoxValues(T all)
            : this(all, all, all, all)
        {

        }

        public BoxValues(T left,
                         T right,
                         T top,
                         T bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

        public T Left { get; }

        public T Right { get; }

        public T Top { get; }

        public T Bottom { get; }
    }
}
