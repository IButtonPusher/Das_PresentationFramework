using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Util;
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
            _targetRect = ValueRectangle.Empty;
            _view = view;
            _displayMetrics = renderKit.DisplayMetrics;
            _viewState = renderKit.RenderContext.ViewState ?? throw new NullReferenceException();

            //var _ = RefreshLoop();
        }

        public AndroidRenderKit RenderKit { get; }

        protected override void OnDraw(Canvas? canvas)
        {
            if (canvas == null)
                return;

            RenderKit.RenderContext.Canvas = canvas;
            RenderKit.RenderContext.DrawMainElement(_view,
                _targetRect, _viewState);
        }

        protected override void OnMeasure(Int32 widthMeasureSpec,
                                          Int32 heightMeasureSpec)
        {
            //System.Diagnostics.Debug.WriteLine("measure paint view");
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            var w = MeasuredWidth;
            var h = MeasuredHeight;

            var sz = new ValueRenderSize(w, h);
            _measured = RenderKit.MeasureContext.MeasureMainView(_view, sz, _viewState);

            _targetRect = new ValueRectangle(
                0, // X
                0, // Y
                sz.Width / _viewState.ZoomLevel,
                sz.Height/ _viewState.ZoomLevel);
            //_measured.Width, _measured.Height);

            //_targetRect = new ValueRectangle(
            //    _displayMetrics.WidthPixels - _measured.Width, // X
            //    _displayMetrics.HeightPixels - _measured.Height, // Y
            //    _measured.Width, _measured.Height);
        }

        //private async Task RefreshLoop()
        //{
        //    var sleepTime = 0;

        //    while (true)
        //        if (_view.IsChanged)
        //        {
        //            //System.Diagnostics.Debug.WriteLine("paint changed");
        //            _view.AcceptChanges();
        //            RenderKit.MeasureContext.MeasureMainView(_view,
        //                new ValueRenderSize(_measured), _viewState);
        //            Invalidate();
        //            sleepTime = 0;
        //        }
        //        else
        //        {
        //            sleepTime = Math.Min(++sleepTime, 50);
        //            await Task.Delay(sleepTime);
        //            //System.Diagnostics.Debug.WriteLine("paint sleeping");
        //        }

        //    // ReSharper disable once FunctionNeverReturns
        //}

        private readonly DisplayMetrics _displayMetrics;
        private readonly IView _view;
        private readonly IViewState _viewState;
        private Size _measured;
        private ValueRectangle _targetRect;
    }
}