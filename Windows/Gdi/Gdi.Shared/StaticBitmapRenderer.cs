using System;
using System.Drawing;
using Das.Views.Core.Geometry;
using Das.Gdi.Kits;
using Das.Views;
using Gdi.Shared.Static;

namespace Das.Gdi
{
    public static class StaticBitmapRenderer
    {
        public static Bitmap? DoRender(IVisualElement view,
                                       StaticGdiRenderKit renderKit,
                                       //IViewState viewState,
                                       Color backgroundColor,
                                       Double dpi,
                                       ValueRenderSize availableSize)
        {

            var viewState = new StaticViewState(dpi);


            var desired = renderKit.MeasureContext.MeasureMainView(view, availableSize,
                viewState);

            if (!desired.IsValid)
                return default;

            var renderTo = new ValueRectangle(0, 0, desired.Width, desired.Height);

            using (var device = new BaseGdiDevice(backgroundColor, desired))
            {
                if (device.Graphics == null)
                    return default!;

                renderKit.RenderContext.Graphics = device.Graphics;
                renderKit.RenderContext.DrawMainElement(view, renderTo, viewState);

                return device.ToBitmap(backgroundColor);
            }

        }
    }
}
