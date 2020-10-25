using System;
using Android.Views;
using Das.Views;
using Das.Views.Core;
using Das.Views.Core.Writing;
using Das.Views.Rendering;

namespace Das.Xamarin.Android
{
    public class AndroidRenderKit : BaseRenderKit, 
                                    IRenderKit
    {
        public AndroidRenderKit(IViewPerspective viewPerspective,
                                IViewState viewState,
                                IFontProvider<AndroidFontPaint> fontProvider,
                                IWindowManager windowManager)
        {

            MeasureContext = new AndroidMeasureKit(windowManager, fontProvider);
            RenderContext = new AndroidRenderContext(MeasureContext, viewPerspective,
                fontProvider, viewState);

            _containedObjects[typeof(IImageProvider)] = RenderContext;
        }

        public AndroidMeasureKit MeasureContext { get; }
        public AndroidRenderContext RenderContext { get; }


        IMeasureContext IRenderKit.MeasureContext => MeasureContext;

        IRenderContext IRenderKit.RenderContext => RenderContext;
    }
}
