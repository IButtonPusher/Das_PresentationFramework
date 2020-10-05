using System;
using System.Threading.Tasks;
using Android.Util;
using Android.Views;
using Das.Views.Core.Geometry;
using Das.Views.Core.Writing;
using Das.Views.Measuring;
using Size = Das.Views.Core.Geometry.Size;

namespace Das.Xamarin.Android
{
    public class AndroidMeasureKit : BaseMeasureContext
    {
        public AndroidMeasureKit(IWindowManager windowManager)
        {
            _windowManager = windowManager;
        }

        public override ISize ContextBounds
        {
            get
            {
                if (!(_windowManager.DefaultDisplay is {} disp))
                    throw new NullReferenceException();

                var metrics = new DisplayMetrics();
                disp.GetMetrics(metrics);

                return new ValueSize();
            }
        }

        public override Size MeasureString(String s, Font font)
        {
            return Size.Empty;
        }

        private readonly IWindowManager _windowManager;
    }
}