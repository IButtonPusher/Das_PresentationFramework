using System;
using System.Drawing;
using System.Threading.Tasks;
using Das.Views;
using Das.Views.Rendering;
using Rectangle = Das.Views.Core.Geometry.Rectangle;

namespace Das.Gdi
{
    public class BitmapRenderer : IRenderer<Bitmap>
    {
        public BitmapRenderer(IViewHost<Bitmap> viewHost,
                              GdiMeasureContext measureContext,
                              GdiRenderContext renderContext,
                              Color backgroundColor)
        {
            _lock = new Object();
            _viewHost = viewHost;
            _visualHost = viewHost;
            _measureContext = measureContext;
            _renderContext = renderContext;
            _gdiDevice = new GdiDevice(backgroundColor,
                (Int32) _viewHost.AvailableSize.Width,
                (Int32) _viewHost.AvailableSize.Height);
            _renderRect = new Rectangle(0, 0, 1, 1);
        }

        public BitmapRenderer(IViewHost<Bitmap> viewHost,
                              GdiMeasureContext measureContext,
                              GdiRenderContext renderContext)
            : this(viewHost, measureContext, renderContext,
                Color.White)
        {
        }

        public Bitmap DoRender()
        {
            lock (_lock)
            {
                var view = _viewHost.View;
                if (view == null)
                    return default!;

                var available = _visualHost.AvailableSize;
                //_measureContext.ViewState = _viewHost;
                //_measureContext.UpdateContextBounds(available);
                var desired = _measureContext.MeasureMainView(view, available, _viewHost);
                _gdiDevice.Width = Convert.ToInt32(desired.Width);
                _gdiDevice.Height = Convert.ToInt32(desired.Height);
                _renderRect.Size = desired;

                var bmp = _gdiDevice.Run(view, DoRender);
                return bmp;
            }
        }

        public event EventHandler? Rendering;

        private void DoRender(Graphics g, IVisualElement view)
        {
            _renderContext.Graphics = g;
            _renderContext.ViewState = _viewHost;
            _renderContext.DrawElement(view, _renderRect);

            Rendering?.Invoke(this, EventArgs.Empty);
        }

        private readonly GdiDevice _gdiDevice;
        private readonly Object _lock;
        private readonly GdiMeasureContext _measureContext;
        private readonly GdiRenderContext _renderContext;
        private readonly Rectangle _renderRect;
        private readonly IViewHost<Bitmap> _viewHost;
        private readonly IVisualHost<Bitmap> _visualHost;
    }
}