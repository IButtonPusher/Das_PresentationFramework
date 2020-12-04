using System;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Support.V4.View;
using Android.Views;
using Das.Views;
using Das.Views.Controls;
using Das.Views.Core.Drawing;
using Das.Views.Core.Geometry;
using Das.Views.Core.Input;
using Das.Views.Input;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Styles;
using Das.Xamarin.Android.Controls;
using Das.Xamarin.Android.Mvvm;
using Size = Das.Views.Core.Geometry.Size;

namespace Das.Xamarin.Android
{
    public class AndroidView : ViewGroup,
                                      IViewState,
                                      IInputContext,
                                      GestureDetector.IOnGestureListener,
                                      GestureDetector.IOnDoubleTapListener,
                                      View.IOnTouchListener
    {
        // ReSharper disable once UnusedMember.Global
        public AndroidView(IView view,
                           Context context,
                           IWindowManager windowManager,
                           AndroidUiProvider uiProvider)
            : this(view, context, 
                BuildRenderKit(context, view, windowManager, uiProvider, 
                    new BaseStyleContext(new DefaultStyle(), new DefaultColorPalette())),
                uiProvider)
        { }


        public AndroidView(IView view,
                           Context context,
                           AndroidRenderKit renderKit,
                           IUiProvider uiProvider)
            : base(context)
        {
            _loopHandler = new Handler(Looper.MainLooper);
            _measured = Size.Empty;
            _targetRect = ValueRectangle.Empty;
            _view = view;
            _uiProvider = uiProvider;
            //_view.PropertyChanged += OnViewPropertyChanged;

            var viewConfig = ViewConfiguration.Get(context) ?? throw new NotSupportedException();

            _maximumFlingVelocity = viewConfig.ScaledMaximumFlingVelocity;
            _minimumFlingVelocity = viewConfig.ScaledMinimumFlingVelocity;

            RenderKit = renderKit;
            RenderKit.RegisterSurrogate<HtmlPanel>(GetHtmlPanelSurrogate);

            ZoomLevel = renderKit.DisplayMetrics.ScaledDensity;

            System.Diagnostics.Debug.WriteLine("Built android view with display w/h: " + 
                                               renderKit.DisplayMetrics.WidthPixels + ", " + 
                                               renderKit.DisplayMetrics.HeightPixels + " dpi: " + 
                                               renderKit.DisplayMetrics.ScaledDensity);

            _paintView = new AndroidPaintView(context, renderKit, view);
            // ReSharper disable once VirtualMemberCallInConstructor
            AddView(_paintView);

            _inputHandler = new BaseInputHandler(RenderKit.RenderContext, this);

            _gestureDetector = new GestureDetectorCompat(context, this);
            _gestureDetector.SetOnDoubleTapListener(this);

            SetOnTouchListener(this);
        }

        private IVisualSurrogate GetHtmlPanelSurrogate(IVisualElement element)
        {
            System.Diagnostics.Debug.WriteLine("Getting html panel " + System.Threading.Thread.CurrentThread.ManagedThreadId);

            return _uiProvider.Invoke(() =>
            {
                if (!(element is HtmlPanel pnl))
                    throw new InvalidOperationException();

                //var surrogate = new NullViewSurrogate(Context);
                var surrogate = new HtmlSurrogate(pnl, Context, this);
                //var webby = new WebView(Context);

                //var surrogate = new HtmlViewSurrogate(pnl, Context,
                //    this, RenderKit.RenderContext);
                AddView(surrogate);

                surrogate.Disposed += OnSurrogateDispoed;

                _hasSurrogates = true;

                //AddView(webby);

                

                return surrogate;
            });
        }

        private static void OnSurrogateDispoed(IVisualElement obj)
        {
            throw new NotImplementedException();
        }

        private static AndroidRenderKit BuildRenderKit(Context context,
                                                       IView view,
                                                       IWindowManager windowManager,
                                                       AndroidUiProvider uiProvider,
                                                       IStyleContext styleContext)
        {
            var displayMetrics = context.Resources?.DisplayMetrics ?? throw new NullReferenceException();
            

            var viewState = new AndroidViewState(view,displayMetrics);

            var fontProvider = new AndroidFontProvider(displayMetrics);
            return new AndroidRenderKit(new BasePerspective(), viewState,
                fontProvider, windowManager, uiProvider, styleContext, displayMetrics);
        }

        IPoint2D IInputProvider.CursorPosition { get; } = Point2D.Empty;

        Boolean IInputProvider.IsCapsLockOn => false;

        Boolean IInputProvider.AreButtonsPressed(KeyboardButtons button1,
                                                 KeyboardButtons button2)
        {
            return false;
        }

        Boolean IInputProvider.AreButtonsPressed(KeyboardButtons button1,
                                                 KeyboardButtons button2,
                                                 KeyboardButtons button3)
        {
            return false;
        }

        Boolean IInputProvider.AreButtonsPressed(MouseButtons button1,
                                                 MouseButtons button2)
        {
            return false;
        }

        Boolean IInputProvider.AreButtonsPressed(MouseButtons button1,
                                                 MouseButtons button2,
                                                 MouseButtons button3)
        {
            return false;
        }

        Boolean IInputProvider.IsButtonPressed(KeyboardButtons keyboardButton)
        {
            return false;
        }

        Boolean IInputProvider.IsButtonPressed(MouseButtons mouseButton)
        {
            return false;
        }

        Boolean IInputContext.IsMousePresent => false;

        public Double MaximumFlingVelocity => _maximumFlingVelocity;

        public Double MinimumFlingVelocity => _minimumFlingVelocity;

        public Boolean TryCaptureMouseInput(IVisualElement view)
        {
            return _inputHandler.TryCaptureMouseInput(view);
        }

        public Boolean TryReleaseMouseCapture(IVisualElement view)
        {
            return _inputHandler.TryReleaseMouseCapture(view);
        }

        public Boolean OnDoubleTap(MotionEvent? e)
        {
            return false;
        }

        public Boolean OnDoubleTapEvent(MotionEvent? e)
        {
            return false;
        }

        public Boolean OnSingleTapConfirmed(MotionEvent? e)
        {
            return false;
        }


        public Boolean OnDown(MotionEvent? e)
        {
            if (!(e is {} eve))
                return false;

            var pos = new ValuePoint2D(eve.GetX(), eve.GetY());
            _leftButtonWentDown = pos;

            if (_inputHandler.OnMouseInput(new MouseDownEventArgs(
                pos, MouseButtons.Left, this), InputAction.LeftMouseButtonDown))
            {
                Invalidate();
                return true;
            }

            return true;
        }

        public Boolean OnFling(MotionEvent? e1,
                               MotionEvent? e2,
                               Single velocityX,
                               Single velocityY)
        {
            if (e1 == null)
                return false;

            var pos = GetPosition(e1);

            var vx = 0 - velocityX * 0.5;
            var vy = 0 - velocityY * 0.5;

            if (Math.Abs(vx) >= MinimumFlingVelocity ||
                Math.Abs(vy) >= MinimumFlingVelocity)
            {
                var flingArgs = new FlingEventArgs(vx, vy, pos, this);
                _inputHandler.OnMouseInput(flingArgs, InputAction.Fling);
            }

            //var ags = new FlingEventArgs(0 - velocityX * 0.5, 0 - velocityY * 0.5, pos, this);
            //if (_inputHandler.OnMouseInput(ags, InputAction.Fling))
            //{
            //    //Invalidate();
            //}
            
            return true;
        }

        public void OnLongPress(MotionEvent? e)
        {
        }

        public Boolean OnScroll(MotionEvent? e1,
                                MotionEvent? e2,
                                Single distanceX,
                                Single distanceY)
        {
            if (e1 == null || e2 == null)
                return false;

            var start = GetPosition(e1);
            var last = GetPosition(e2);

            var delta = new ValueSize(0 - distanceX, 0 - distanceY);

            var dragArgs = new Das.Views.Input.DragEventArgs(start, last, delta,
                _leftButtonWentDown != null ? MouseButtons.Left : MouseButtons.Right,
                this);
            if (_inputHandler.OnMouseInput(dragArgs, InputAction.MouseDrag))
            {
                //Invalidate();
            }

            return true;
        }

        public void OnShowPress(MotionEvent? e)
        {
        }

        public Boolean OnSingleTapUp(MotionEvent? e)
        {
            if (e == null)
                return false;

            var pos = GetPosition(e);
            if (_inputHandler.OnMouseInput(new MouseUpEventArgs(
                pos, _leftButtonWentDown, MouseButtons.Left, this), 
                InputAction.LeftMouseButtonUp))
                Invalidate();
            return false;
        }

        public Boolean OnTouch(View? v, MotionEvent? e)
        {
            if (e?.Action == MotionEventActions.Up)
            {
                // gesture detector not detecting anything for when you lift the finger after dragging
                var pos = GetPosition(e);
                if (_inputHandler.OnMouseInput(new MouseUpEventArgs(pos, 
                    _leftButtonWentDown, MouseButtons.Left, this), InputAction.LeftMouseButtonUp))
                {
                    //Invalidate();
                }
            }

            return _gestureDetector.OnTouchEvent(e);
        }


        public T GetStyleSetter<T>(StyleSetter setter,
                                   IVisualElement element)
        {
            return _view.StyleContext.GetStyleSetter<T>(setter, element);
        }

        public T GetStyleSetter<T>(StyleSetter setter,
                                   StyleSelector selector,
                                   IVisualElement element)
        {
            return _view.StyleContext.GetStyleSetter<T>(setter, selector, element);
        }

        public void RegisterStyleSetter(IVisualElement element, 
                                        StyleSetter setter, 
                                        Object value)
        {
            _view.StyleContext.RegisterStyleSetter(element, setter, value);
        }
        public void RegisterStyleSetter(IVisualElement element, 
                                        StyleSetter setter, 
                                        StyleSelector selector, 
                                        Object value)
        {
            _view.StyleContext.RegisterStyleSetter(element, setter, selector, value);
        }

        public IColorPalette ColorPalette => _view.StyleContext.ColorPalette;

        public IColor GetCurrentAccentColor()
        {
            return _view.StyleContext.GetCurrentAccentColor();
        }

        public Double ZoomLevel { get; }

        public AndroidRenderKit RenderKit { get; }

        public override Boolean OnGenericMotionEvent(MotionEvent? e)
        {
            _gestureDetector.OnTouchEvent(e);
            return base.OnGenericMotionEvent(e);
        }

        public override Boolean OnTouchEvent(MotionEvent? e)
        {
            _gestureDetector.OnTouchEvent(e);
            return base.OnTouchEvent(e);
        }


        public sealed override void SetOnTouchListener(IOnTouchListener? l)
        {
            base.SetOnTouchListener(l);
        }

        protected override void OnLayout(Boolean changed, 
                                         Int32 left, 
                                         Int32 top, 
                                         Int32 right, 
                                         Int32 bottom)
        {
            if (_hasSurrogates)
            {
                RenderKit.RefreshRenderContext.DrawMainElement(_view,
                    new ValueRectangle(0, 0, _targetRect.Width, _targetRect.Height), this);

                //RenderKit.RefreshRenderContext.DrawElement(_view,
                //    new ValueRenderRectangle(0, 0, _measured.Width, _measured.Height,
                //        ValuePoint2D.Empty));
            }

            var count = ChildCount;
            for (var c = 0; c < count; c++)
            {
                var current = GetChildAt(c);

                if (current == null)
                    continue;

                if (current is IVisualSurrogate surrogate)
                {
                    var wants = RenderKit.RenderContext.TryGetElementBounds(surrogate);
                    if (wants != null)
                    {
                        left = Convert.ToInt32(wants.Left * ZoomLevel);
                        top = Convert.ToInt32(wants.Top * ZoomLevel);
                        right = Convert.ToInt32(wants.Right * ZoomLevel);
                        bottom = Convert.ToInt32(wants.Bottom * ZoomLevel);
                    }
                }


                current.Layout(left, top, right, bottom);
            }
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
                sz.Height/ ZoomLevel);

            var count = ChildCount;

            for (var c = 0; c < count; c++)
            {
                var current = GetChildAt(c);

                if (current is IVisualSurrogate)
                {
                    current.Measure(Convert.ToInt32(_targetRect.Width),
                        Convert.ToInt32(_targetRect.Height));
                }
                else
                    current?.Measure(widthMeasureSpec, heightMeasureSpec);
            }

            _measured = new ValueSize(sz.Width, sz.Height);
            RenderKit.MeasureContext.MeasureMainView(_view, sz, this);

            if (_refreshLoopCount == 0)
            {
                _refreshLoopCount++;
                var _ = RefreshLoop();
            }
        }

        private static ValuePoint2D GetPosition(MotionEvent eve)
        {
            return new ValuePoint2D(eve.GetX(), eve.GetY());
        }


        private async Task RefreshLoop()
        {
            var sleepTime = 0;

            while (true)
                if (_view.IsChanged || _view.StyleContext.IsChanged)
                {
                    _view.StyleContext.AcceptChanges();
                    RenderKit.MeasureContext.MeasureMainView(_view,
                        new ValueRenderSize(_measured), this);

                    _paintView.Invalidate();
                    Invalidate();
                    sleepTime = 0;
                    await Task.Yield();
                }
                else
                {
                    sleepTime = Math.Min(++sleepTime, 50);

                    if (sleepTime <= 5)
                        System.Diagnostics.Debug.WriteLine("SKIP FRAME");
                    await Task.Delay(sleepTime);
                }

            // ReSharper disable once FunctionNeverReturns
        }

        private readonly GestureDetectorCompat _gestureDetector;

        private readonly BaseInputHandler _inputHandler;
        private readonly Int32 _maximumFlingVelocity;
        private readonly Int32 _minimumFlingVelocity;
        private readonly IView _view;
        private readonly IUiProvider _uiProvider;
        private ValuePoint2D? _leftButtonWentDown;

        private Int32 _refreshLoopCount;

        // ReSharper disable once NotAccessedField.Local
        private Handler _loopHandler;
        private Boolean _hasSurrogates;

        private Size _measured;
        private ValueRectangle _targetRect;
        private readonly AndroidPaintView _paintView;
    }
}