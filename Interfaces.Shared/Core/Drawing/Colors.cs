using System;

namespace Das.Views.Core.Drawing
{
    public enum Colors
    {
        Black = (255 << 24),
        Transparent = 0,
        Red = 255 + (255 << 24),
        White = Int32.MaxValue
    }
}