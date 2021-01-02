using System;
using Das.ViewModels.Collections;
using Das.Views.Colors;
using Das.Views.Core.Drawing;

namespace Das.Views.Styles
{
    public class ColorPalette : IColorPalette
    {
        public ColorPalette(IBrush primary, 
                            IBrush onPrimary,
                            IBrush onSurface, 
                            IBrush background, 
                            IBrush accent,
                            IBrush onBackground,
                            IBrush surface)
        {
            Accent = accent;
            OnBackground = onBackground;
            Surface = surface;
            Background = background;
            Primary = primary;
            OnSurface = onSurface;
            OnPrimary = onPrimary;

            _cachedAlphas = new DoubleConcurrentDictionary<ColorType, Double, IBrush>();
        }

        public IBrush Primary { get; }

        public IBrush Accent { get; }

        public IBrush Background { get; }

        public IBrush OnBackground { get; }

        public IBrush Surface { get; }

        public IBrush OnSurface { get; }

        public IBrush OnPrimary { get; }

        private readonly DoubleConcurrentDictionary<ColorType, Double, IBrush> _cachedAlphas;

        public IBrush GetAlpha(ColorType type,
                               Double opacity)
        {
            return _cachedAlphas.GetOrAdd(type, opacity, BuildAlphaBrush);
        }

        private IBrush BuildAlphaBrush(ColorType type,
                                       Double opacity)
        {
            var brush = GetBrush(type);
            var alphad = brush.GetWithOpacity(opacity);
            return alphad;
        }

        private IBrush GetBrush(ColorType type)
        {
            switch (type)
            {
                case ColorType.Primary:
                    return Primary;

                case ColorType.OnPrimary:
                    return OnPrimary;

                case ColorType.Accent:
                    return Accent;

                case ColorType.Background:
                    return Background;

                case ColorType.OnBackground:
                    return OnBackground;

                case ColorType.Surface:
                    return Surface;

                case ColorType.OnSurface:
                    return OnSurface;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

    }
}
