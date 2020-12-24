using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Util;
using Das.Views;
using Das.Views.Core.Writing;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Xamarin.Android
{
    public class AndroidFontProvider //: IFontProvider<AndroidFontPaint>
    {
        public AndroidFontProvider(DisplayMetrics displayMetrics,
                                   IStyleContext styleContext)
        {
            _fontLock = new Object();

            _displayMetrics = displayMetrics;
            //_styleContext = styleContext;
            _fonts = new Dictionary<IFont, AndroidFontPaint>();
            _visualFonts = new Dictionary<IVisualElement, Dictionary<IFont, AndroidFontPaint>>();
        }

        public void RemoveElement(IVisualElement visual)
        {
            lock (_fontLock)
            {
                if (_visualFonts.TryGetValue(visual, out var myFonts))
                {
                    _visualFonts.Remove(visual);
                    foreach (var kvp in myFonts)
                    {
                        kvp.Value.Dispose();
                    }
                }
            }
        }

        public AndroidFontPaint GetRenderer(IFont font,
                                            IVisualLineage visualLineage)
        {
            var currentVisual = visualLineage.PeekVisual();

            lock (_fontLock)
            {
                AndroidFontPaint painter;
                if (currentVisual != null)
                {
                    if (!_visualFonts.TryGetValue(currentVisual, out var myFonts))
                    {
                        myFonts = new Dictionary<IFont, AndroidFontPaint>();
                        _visualFonts[currentVisual] = myFonts;
                    }

                    if (!myFonts.TryGetValue(font, out painter))
                    {
                        painter = new AndroidFontPaint(font, _displayMetrics, true);
                        myFonts[font] = painter;
                    }

                    return painter;
                }

                if (!_fonts.TryGetValue(font, out painter))
                {
                    painter = new AndroidFontPaint(font, _displayMetrics, false); 
                    _fonts[font] = painter;
                }

                return painter;
            }
        }

        private readonly DisplayMetrics _displayMetrics;
        //private readonly IStyleContext _styleContext;

        private readonly Dictionary<IVisualElement, Dictionary<IFont, AndroidFontPaint>> _visualFonts;
        private readonly Dictionary<IFont, AndroidFontPaint> _fonts;
        private readonly Object _fontLock;
    }
}