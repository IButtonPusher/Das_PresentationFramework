using System;

namespace Das.Views.Core.Drawing
{
    public interface IPen
    {
        IColor Color { get; }
        Int32 Thickness { get; }
    }
}