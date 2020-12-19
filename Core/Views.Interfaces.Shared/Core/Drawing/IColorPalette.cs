using System;

namespace Das.Views.Core.Drawing
{
    public interface IColorPalette
    {
        IBrush Accent { get; }

        IBrush Background { get; }
    }
}
