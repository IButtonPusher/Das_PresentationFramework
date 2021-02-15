using System;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Views;
using Das.Container;
using Das.Views;
using Das.Views.Colors;
using Das.Views.Controls;

using Das.Views.Core.Geometry;
using Das.Views.Input;
using Das.Views.Rendering;
using Das.Views.Styles;
using Das.Xamarin.Android.Controls;
using Das.Xamarin.Android.Images;
using Das.Xamarin.Android.Input;


namespace Das.Xamarin.Android
{
    /// <summary>
    /// ViewGroup that contains the AndroidPaintView for 'normal' visuals as well as surrogated visuals,
    /// when applicable
    /// </summary>
    /// <see cref="AndroidPaintView"/>
    public class AndroidView : ViewGroup
    {
        // ReSharper disable once UnusedMember.Global
        public AndroidView(IVisualElement view,
                           Context context,
                           IWindowManager windowManager,
                           IUiProvider uiProvider,
                           IThemeProvider? themeProvider,
                           IResolver? resolver)
            : this(view, context,
                BuildRenderKit(context, windowManager, uiProvider,
                    themeProvider ?? BaselineThemeProvider.Instance,
                    resolver ?? new BaseResolver()),
                uiProvider)
        {
        }


        public AndroidView(IVisualElement view,
                           Context context,
                           AndroidRenderKit renderKit,
                           IUiProvider uiProvider)
            : base(context)
        {
            _loopHandler = new Handler(Looper.MainLooper);
            _measured = Size.Empty;
            _targetRect = ValueRectangle.Empty;
            _view = view;
            _surrogates = new AndroidSurrogateProvider(renderKit, uiProvider, this);

            RenderKit = renderKit;
            _viewState = renderKit.ViewState;


            ZoomLevel = renderKit.DisplayMetrics.ScaledDensity;

            //System.Diagnostics.Debug.WriteLine("Built android view with display w/h: " +
            //                                   renderKit.DisplayMetrics.WidthPixels + ", " +
            //                                   renderKit.DisplayMetrics.HeightPixels + " dpi: " +
            //                                   renderKit.DisplayMetrics.ScaledDensity);


            _paintView = new AndroidPaintView(context, renderKit, view);
            AddView(_paintView);

            var inputHandler = new BaseInputHandler(RenderKit.RenderContext);
            _inputContext = new AndroidInputContext(this, context, inputHandler, _viewState);

            renderKit.Container.ResolveTo(this);
        }

        public sealed override void AddView(View? child)
        {
            base.AddView(child);
        }

        public Double ZoomLevel { get; }

        public AndroidRenderKit RenderKit { get; }

        public override Boolean OnGenericMotionEvent(MotionEvent? e)
        {
            _inputContext.OnGenericMotionEvent(e);
            return base.OnGenericMotionEvent(e);
        }

        public override Boolean OnTouchEvent(MotionEvent? e)
        {
            _inputContext.OnTouchEvent(e);
            return base.OnTouchEvent(e);
        }


        protected override void OnLayout(Boolean changed,
                                         Int32 left,
                                         Int32 top,
                                         Int32 right,
                                         Int32 bottom)
        {
            var hasSurrogates = ChildCount > 1;

            if (hasSurrogates)
            {
                RenderKit.RefreshRenderContext.DrawMainElement(_view,
                    new ValueRectangle(0, 0, _targetRect.Width, _targetRect.Height), _viewState);
                _view.InvalidateArrange();
            }

            //WriteLine("AndroidView->OnLayout surrogates: " + hasSurrogates +
            //          " view needs arrange: " + _view.IsRequiresArrange);

            var count = ChildCount;
            for (var c = 0; c < count; c++)
            {
                var current = GetChildAt(c);

                if (current == null)
                {
                    //WriteLine("Current child is null!");
                    continue;
                }

                if (hasSurrogates && current is IVisualSurrogate surrogate)
                {
                    var wants = RenderKit.RenderContext.TryGetElementBounds(surrogate.ReplacingVisual);
                    if (wants != null)
                    {
                        left = Convert.ToInt32(wants.Left * ZoomLevel);
                        top = Convert.ToInt32(wants.Top * ZoomLevel);
                        right = Convert.ToInt32(wants.Right * ZoomLevel);
                        bottom = Convert.ToInt32(wants.Bottom * ZoomLevel);
                    }
                    else 
                        continue;

                    //WriteLine("[OKYN] Calling layout on surrogate " + current + " ltrb: " + left + "," + top + "," + right +
                    //          "," + bottom);
                }


                current.Layout(left, top, right, bottom);
            }
        }

        // ReSharper disable once UnusedMember.Local
        private static void WriteLine(String msg)
        {
            System.Diagnostics.Debug.WriteLine("[OKYN] " + msg);
        }

        protected override void Dispose(Boolean disposing)
        {
            base.Dispose(disposing);
            _loopHandler.Dispose();
        }

        protected override void OnMeasure(Int32 widthMeasureSpec,
                                          Int32 heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            var w = MeasuredWidth;
            var h = MeasuredHeight;

            var sz = new ValueRenderSize(w, h);

            _targetRect = new ValueRectangle(
                0, // X
                0, // Y
                sz.Width / ZoomLevel,
                sz.Height / ZoomLevel);

            var count = ChildCount;

            for (var c = 0; c < count; c++)
            {
                var current = GetChildAt(c);

                if (current is IVisualSurrogate)
                    current.Measure(Convert.ToInt32(_targetRect.Width),
                        Convert.ToInt32(_targetRect.Height));
                else
                    current?.Measure(widthMeasureSpec, heightMeasureSpec);
            }

            _measured = new ValueSize(sz.Width, sz.Height);
            RenderKit.MeasureContext.MeasureMainView(_view, sz, _viewState);

            if (_refreshLoopCount == 0)
            {
                _refreshLoopCount++;
                var _ = RefreshLoop();
            }
        }


        private static AndroidRenderKit BuildRenderKit(Context context,
                                                       IWindowManager windowManager,
                                                       IUiProvider uiProvider,
                                                       IThemeProvider themeProvider,
                                                       IResolver resolver)
        {
            var displayMetrics = context.Resources?.DisplayMetrics ?? throw new NullReferenceException();


            var viewState = new AndroidViewState(displayMetrics, themeProvider);

            var fontProvider = new AndroidFontProvider(displayMetrics);
            return new AndroidRenderKit(new BasePerspective(), viewState,
                fontProvider, windowManager, uiProvider, themeProvider, displayMetrics,
                resolver, new AndroidImageProvider(displayMetrics));
        }


        private async Task RefreshLoop()
        {
            while (true)
            {
                var willInvalidate = false;

                if (_view.IsRequiresMeasure)
                {
                    RenderKit.MeasureContext.MeasureMainView(_view,
                        new ValueRenderSize(_measured), _viewState);

                    willInvalidate = true;
                }
                else if (_view.IsRequiresArrange || _inputContext.IsInteracting)
                    willInvalidate = true;

                if (willInvalidate)
                {
                    if (ChildCount > 1)
                        Invalidate();
                    //WriteLine("Invalidating paint view");
                    _paintView.Invalidate();
                    
                    _inputContext.SleepTime = 0;
                    await Task.Yield();
                }
                else
                {
                    //if (_inputContext.SleepTime == 0)
                    //    WriteLine("frame skipped!");

                    _inputContext.SleepTime = Math.Min(++_inputContext.SleepTime, 50);

                    await Task.Delay(_inputContext.SleepTime);
                }
            }

            // ReSharper disable once FunctionNeverReturns
        }

        private readonly AndroidInputContext _inputContext;
        private readonly AndroidPaintView _paintView;

        // ReSharper disable once NotAccessedField.Local
        private readonly AndroidSurrogateProvider _surrogates;

        private readonly IVisualElement _view;

        private readonly Handler _loopHandler;

        private Size _measured;
        private readonly IViewState _viewState;
        private Int32 _refreshLoopCount;
        private ValueRectangle _targetRect;
    }
}