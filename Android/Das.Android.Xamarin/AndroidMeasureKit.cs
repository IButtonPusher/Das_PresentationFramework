using System;
using System.Threading.Tasks;
using Android.Util;
using Android.Views;
using Das.Views.Controls;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Measuring;
using Size = Das.Views.Core.Geometry.Size;

namespace Das.Xamarin.Android
{
    public class AndroidMeasureKit : BaseMeasureContext
    {
        public AndroidMeasureKit(IWindowManager windowManager,
                                 IFontProvider<AndroidFontPaint> fontProvider,
                                 IVisualSurrogateProvider surrogateProvider)
        : base(surrogateProvider)
        {
            _windowManager = windowManager;
            _fontProvider = fontProvider;
        }

        public override ISize ContextBounds
        {
            get
            {
                if (!(_windowManager.DefaultDisplay is {} disp))
                    throw new NullReferenceException();

                var metrics = new DisplayMetrics();
                disp.GetMetrics(metrics);

                return new ValueSize(metrics.WidthPixels, metrics.HeightPixels);
            }
        }

        public override Size MeasureString(String s, 
                                           IFont font)
        {
            var renderer = _fontProvider.GetRenderer(font);
            return renderer.MeasureString(s);
        }

        private readonly IWindowManager _windowManager;
        private readonly IFontProvider<AndroidFontPaint> _fontProvider;
    }
}