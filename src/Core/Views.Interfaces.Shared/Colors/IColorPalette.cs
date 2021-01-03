using System;
using System.Collections.Generic;
using Das.Views.Colors;

namespace Das.Views.Core.Drawing
{
    public interface IColorPalette: IEnumerable<KeyValuePair<ColorType, IBrush>>
    {
        IBrush Primary { get; }

        IBrush PrimaryVariant { get; }

        IBrush Secondary { get; }

        IBrush SecondaryVariant { get; }
        
        IBrush Background { get; }

        IBrush Surface { get; }

        IBrush Error { get; }
        
        IBrush OnPrimary { get; }
        
        IBrush OnSecondary { get; }

        IBrush OnBackground {get;}

        IBrush OnSurface { get; }

        IBrush OnError { get; }

        //IBrush Accent { get; }


        IBrush GetAlpha(ColorType type,
                        Double opacity);
    }
}
