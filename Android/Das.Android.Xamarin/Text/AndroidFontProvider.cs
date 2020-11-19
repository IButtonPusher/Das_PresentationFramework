using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Util;
using Das.Views.Core.Writing;

namespace Das.Xamarin.Android
{
    public class AndroidFontProvider : IFontProvider<AndroidFontPaint>
    {
        private readonly DisplayMetrics _displayMetrics;

        public AndroidFontProvider(DisplayMetrics displayMetrics)
        {
            _displayMetrics = displayMetrics;
            _fonts = new Dictionary<IFont, AndroidFontPaint>();
        }

        public AndroidFontPaint GetRenderer(IFont font)
        {
            if (!_fonts.TryGetValue(font, out var painter))
            {
                painter = new AndroidFontPaint(font, _displayMetrics, true);
                _fonts[font] = painter;
            }

            return painter;
        }

        private readonly Dictionary<IFont, AndroidFontPaint> _fonts;
    }
}