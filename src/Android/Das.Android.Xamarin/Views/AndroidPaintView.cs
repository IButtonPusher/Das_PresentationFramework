using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Das.Views;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;
using View = Android.Views.View;

namespace Das.Xamarin.Android
{
    /// <summary>
    /// Android view that measures and arranges DPF visuals that are not delegated to surrogates
    /// </summary>
    public class AndroidPaintView : View
    {
        public AndroidPaintView(Context? context,
                                AndroidRenderKit renderKit,
                                IVisualElement view)
            : base(context)
        {
            _renderContext = renderKit.RenderContext;
            _measureContext = renderKit.MeasureContext;
            _targetRect = ValueRectangle.Empty;
            _view = view;
            
            _viewState = renderKit.RenderContext.ViewState ?? throw new NullReferenceException();
        }

        protected override void OnDraw(Canvas? canvas)
        {
            //System.Diagnostics.Debug.WriteLine("[OKYN] AndroidPaintView->OnDraw( " + canvas + ")");

            if (canvas == null)
                return;

            _renderContext.Canvas = canvas;
            _renderContext.DrawMainElement(_view,
                _targetRect, _viewState);
        }

        protected override void OnMeasure(Int32 widthMeasureSpec,
                                          Int32 heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            var w = MeasuredWidth;
            var h = MeasuredHeight;

            var sz = new ValueRenderSize(w, h);
            
            _measureContext.MeasureMainView(_view, sz, _viewState);

            _targetRect = new ValueRectangle(
                0, // X
                0, // Y
                sz.Width / _viewState.ZoomLevel,
                sz.Height/ _viewState.ZoomLevel);
        }

        private readonly AndroidMeasureKit _measureContext;
        private readonly AndroidRenderContext _renderContext;

        
        private readonly IVisualElement _view;
        private readonly IViewState _viewState;
        
        private ValueRectangle _targetRect;
    }
}