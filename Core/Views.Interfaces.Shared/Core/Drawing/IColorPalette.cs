using System;

namespace Das.Views.Core.Drawing
{
    public interface IColorPalette
    {
        IColor Accent { get; }

        IColor Background { get; }
    }
}
