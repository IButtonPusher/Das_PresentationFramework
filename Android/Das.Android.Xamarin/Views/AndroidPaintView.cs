using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Das.Views.Core.Geometry;
using Das.Views.Panels;
using Das.Views.Rendering;
using Size = Das.Views.Core.Geometry.Size;

namespace Das.Xamarin.Android
{
    public class AndroidPaintView : View
    {
        public AndroidPaintView(Context? context, 
                                AndroidRenderKit renderKit, 
                                IView view) 
            : base(context)
        {
            RenderKit = renderKit;
            _measured = Size.Empty;
            _view = view;
            _viewState = renderKit.RenderContext.ViewState ?? throw new NullReferenceException();

            var _ = RefreshLoop();
        }

        private async Task RefreshLoop()
        {
            while (true)
                if (_view.IsChanged)
                {
                    _view.AcceptChanges();
                    RenderKit.MeasureContext.MeasureMainView(_view,
                        new ValueRenderSize(_measured), _viewState);
                    Invalidate();
                }
                else
                    await Task.Delay(50);

            // ReSharper disable once FunctionNeverReturns
        }

        public AndroidRenderKit RenderKit { get; }
        private readonly IView _view;
        private readonly IViewState _viewState;
        private Size _measured;

        protected override void OnMeasure(Int32 widthMeasureSpec,
                                          Int32 heightMeasureSpec)
        {
            System.Diagnostics.Debug.WriteLine("measure paint view");
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            var w = MeasuredWidth;
            var h = MeasuredHeight;

            var sz = new ValueRenderSize(w, h);
            _measured = RenderKit.MeasureContext.MeasureMainView(_view, sz, _viewState);
        }

        protected override void OnDraw(Canvas? canvas)
        {
            System.Diagnostics.Debug.WriteLine("draw paint view");

            if (canvas == null)
                return;

            RenderKit.RenderContext.Canvas = canvas;
            RenderKit.RenderContext.DrawMainElement(_view, 
                new ValueRectangle(0, 0, _measured), _viewState);
        }

       
    }
}