using System;

namespace Das.Views.Core.Drawing
{
    public interface IColor
    {
        Byte A { get; }

        Byte B { get; }

        Byte R { get; }

        Byte G { get; }
    }
}