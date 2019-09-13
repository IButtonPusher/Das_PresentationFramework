using System;
using System.Drawing;
using Das.Views;
using Das.Views.Rendering;
using Rectangle = Das.Views.Core.Geometry.Rectangle;

namespace Das.Gdi
{
    public class BitmapRenderer : IRenderer<Bitmap>
    {
        private readonly IViewHost<Bitmap> _viewHost;
        private readonly GdiMeasureContext _measureContext;
        private readonly GdiRenderContext _renderContext;
        private readonly GdiDevice _gdiDevice;
        private readonly Rectangle _renderRect;

        public BitmapRenderer(IViewHost<Bitmap> viewHost, GdiMeasureContext measureContext,
            GdiRenderContext renderContext, Color backgroundColor)
        {
            _viewHost = viewHost;
            _measureContext = measureContext;
            _renderContext = renderContext;
            _gdiDevice = new GdiDevice(backgroundColor,
                (Int32) _viewHost.AvailableSize.Width,
                (Int32) _viewHost.AvailableSize.Height);
            _renderRect = new Rectangle(0, 0, 1, 1);
        }

        public BitmapRenderer(IViewHost<Bitmap> viewHost, GdiMeasureContext measureContext,
            GdiRenderContext renderContext) : this(viewHost, measureContext, renderContext,
            Color.White)
        {
        }

        public Bitmap DoRender()
        {
            var view = _viewHost.View;
            if (view == null)
                return default;

            var available = _viewHost.AvailableSize;
            _measureContext.ViewState = _viewHost;

            var desired = _measureContext.MeasureElement(view, available);
            _gdiDevice.Width = Convert.ToInt32(desired.Width);
            _gdiDevice.Height = Convert.ToInt32(desired.Height);
            _renderRect.Size = desired;

            var bmp = _gdiDevice.Run(g => DoRender(g, view));
            return bmp;
        }

        public event EventHandler Rendering;

        private void DoRender(Graphics g, IVisualElement view)
        {
            _renderContext.Graphics = g;
            _renderContext.ViewState = _viewHost;
            _renderContext.DrawElement(view, _renderRect);

            Rendering?.Invoke(this, EventArgs.Empty);
        }
    }
}