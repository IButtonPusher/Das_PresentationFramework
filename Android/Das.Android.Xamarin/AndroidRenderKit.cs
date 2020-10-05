using System;
using Android.Views;
using Das.Views;
using Das.Views.Core.Writing;
using Das.Views.Rendering;

namespace Das.Xamarin.Android
{
    public class AndroidRenderKit : IRenderKit
    {
        public AndroidRenderKit(IViewPerspective viewPerspective,
                                IViewState viewState,
                                IFontProvider<AndroidFontPaint> fontProvider,
                                IWindowManager windowManager)
        {

            MeasureContext = new AndroidMeasureKit(windowManager);
            RenderContext = new AndroidRenderContext(MeasureContext, viewPerspective,
                fontProvider, viewState);
        }

        public AndroidMeasureKit MeasureContext { get; }
        public AndroidRenderContext RenderContext { get; }


        IMeasureContext IRenderKit.MeasureContext => MeasureContext;

        IRenderContext IRenderKit.RenderContext => RenderContext;
    }
}
