﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.ViewModels.Collections;
using Das.Views.Colors;
using Das.Views.Core.Drawing;

namespace Das.Views.Styles
{
    public class ColorPalette : IColorPalette
    {
        public ColorPalette(IBrush primary,
                            IBrush primaryVariant,
                            IBrush secondary,
                            IBrush secondaryVariant,
                            IBrush background,
                            IBrush surface,
                            IBrush error,
                            IBrush onPrimary,
                            IBrush onSecondary,
                            IBrush onBackground,
                            IBrush onSurface,
                            IBrush onError)
        {
            _cachedAlphas = new DoubleConcurrentDictionary<ColorType, Double, IBrush>();

            Primary = primary;
            PrimaryVariant = primaryVariant;
            Secondary = secondary;
            SecondaryVariant = secondaryVariant;
            Background = background;
            OnSecondary = onSecondary;
            OnBackground = onBackground;
            Surface = surface;
            Error = error;
            OnSurface = onSurface;
            OnError = onError;
            OnPrimary = onPrimary;
        }

        public static readonly ColorPalette Baseline = new ColorPalette(
           primary: Hex("#6200EE"),
           primaryVariant: Hex("#3700B3"),
           secondary:Hex("#03DAC6"),
           secondaryVariant:Hex("#018786"),
           background: Hex("#FFFFFF"),
           surface: Hex("#FFFFFF"),
           error: Hex("#B00020"),
           onPrimary: Hex("#FFFFFF"),
           onSecondary: Hex("#000000"),
           onBackground: Hex("#000000"),
           onSurface: Hex("#000000"),
           onError: Hex("#FFFFFF"));

        public static readonly ColorPalette DefaultDark = new ColorPalette(
           primary: Hex("#BB86FC"), //
           primaryVariant: Hex("#3700BC"),
           secondary: Hex("#03DAC6"), //
           secondaryVariant: Hex("#03DAC6"),
           background: Hex("#121212"), //
           surface: Hex("#121212"), //
           error: Hex("#CF6679"), //
           onPrimary: Hex("#000000"),
           onSecondary: Hex("#000000"),
           onBackground: Hex("#FFFFFF"),
           onSurface: Hex("#FFFFFF"),
           onError: Hex("#000000"));

        private static IBrush Hex(String hex) => new SolidColorBrush(hex);

        public IBrush Primary { get; }

        public IBrush PrimaryVariant { get; }

        public IBrush Secondary { get; }

        public IBrush SecondaryVariant { get; }

        public IBrush Background { get; }

        public IBrush OnSecondary { get; }

        public IBrush OnBackground { get; }

        public IBrush Surface { get; }

        public IBrush Error { get; }

        public IBrush OnSurface { get; }

        public IBrush OnError { get; }

        public IBrush OnPrimary { get; }

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

                case ColorType.PrimaryVariant:
                    return PrimaryVariant;

                case ColorType.Secondary:
                    return Secondary;

                case ColorType.SecondaryVariant:
                    return SecondaryVariant;

                case ColorType.Background:
                    return Background;

                case ColorType.Surface:
                    return Surface;

                case ColorType.Error:
                    return Error;


                case ColorType.OnPrimary:
                    return OnPrimary;

                case ColorType.OnSecondary:
                    return OnSecondary;

                case ColorType.OnBackground:
                    return OnBackground;

                case ColorType.OnSurface:
                    return OnSurface;

                case ColorType.OnError:
                    return OnSurface;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private readonly DoubleConcurrentDictionary<ColorType, Double, IBrush> _cachedAlphas;

        public IEnumerator<KeyValuePair<ColorType, IBrush>> GetEnumerator()
        {
            foreach (ColorType ct in Enum.GetValues(typeof(ColorType)))
                yield return new KeyValuePair<ColorType, IBrush>(ct, GetBrush(ct));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
