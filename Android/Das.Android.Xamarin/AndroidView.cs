using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.View;
using Android.Views;
using Das.Views.Core.Geometry;
using Das.Views.Core.Input;
using Das.Views.Input;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Styles;
using Debug = System.Diagnostics.Debug;

namespace Das.Xamarin.Android
{
    public class AndroidView : View,
                               IViewState,
                               IInputContext,
                               GestureDetector.IOnGestureListener,
                               GestureDetector.IOnDoubleTapListener,
                               View.IOnTouchListener
    {
        // ReSharper disable once UnusedMember.Global
        public AndroidView(IView view,
                           Context context,
                           IWindowManager windowManager)
            : base(context)
        {
            _loopHandler = new Handler(Looper.MainLooper);
            _measured = Size.Empty;
            _view = view;
            _view.PropertyChanged += OnViewPropertyChanged;

            var viewConfig = ViewConfiguration.Get(context) ?? throw new NotSupportedException();

            _maximumFlingVelocity = viewConfig.ScaledMaximumFlingVelocity;
            _minimumFlingVelocity = viewConfig.ScaledMinimumFlingVelocity;

            var displayMetrics = context.Resources?.DisplayMetrics ?? throw new NullReferenceException();

            var fontProvider = new AndroidFontProvider(displayMetrics);

            RenderKit = new AndroidRenderKit(new BasePerspective(), this,
                fontProvider, windowManager);
            _inputHandler = new BaseInputHandler(RenderKit.RenderContext);

            _gestureDetector = new GestureDetectorCompat(context, this);
            _gestureDetector.SetOnDoubleTapListener(this);

            SetOnTouchListener(this);


            Task.Run(RefreshLoop).ConfigureAwait(false);
        }


        public AndroidView(IView view,
                           Context context,
                           AndroidRenderKit renderKit)
            : base(context)
        {
            _loopHandler = new Handler(Looper.MainLooper);
            _measured = Size.Empty;
            _view = view;
            _view.PropertyChanged += OnViewPropertyChanged;

            var viewConfig = ViewConfiguration.Get(context) ?? throw new NotSupportedException();

            _maximumFlingVelocity = viewConfig.ScaledMaximumFlingVelocity;
            _minimumFlingVelocity = viewConfig.ScaledMinimumFlingVelocity;

            RenderKit = renderKit;

            _inputHandler = new BaseInputHandler(RenderKit.RenderContext);

            _gestureDetector = new GestureDetectorCompat(context, this);
            _gestureDetector.SetOnDoubleTapListener(this);

            SetOnTouchListener(this);

            var _ = RefreshLoop();
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
            var ags = new FlingEventArgs(0 - velocityX * 0.5, 0 - velocityY * 0.5, pos, this);
            if (_inputHandler.OnMouseInput(ags, InputAction.Fling))
                Invalidate();


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

            var dragArgs = new Views.Input.DragEventArgs(start, last, delta,
                _leftButtonWentDown != null ? MouseButtons.Left : MouseButtons.Right,
                this);
            if (_inputHandler.OnMouseInput(dragArgs, InputAction.MouseDrag))
                Invalidate();

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
                pos, MouseButtons.Left, this), InputAction.LeftMouseButtonUp))
                Invalidate();
            return false;
        }

        public Boolean OnTouch(View? v, MotionEvent? e)
        {
            if (e?.Action == MotionEventActions.Up)
            {
                // gesture detector not detecting anything for when you lift the finger after dragging
                var pos = GetPosition(e);
                if (_inputHandler.OnMouseInput(new MouseUpEventArgs(
                    pos, MouseButtons.Left, this), InputAction.LeftMouseButtonUp))
                    Invalidate();
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

        public Double ZoomLevel => 1;

        public AndroidRenderKit RenderKit { get; }

        private static ValuePoint2D GetPosition(MotionEvent eve)
        {
            return new ValuePoint2D(eve.GetX(), eve.GetY());
        }


        protected override void OnDraw(Canvas? canvas)
        {
            if (canvas == null)
                return;

            RenderKit.RenderContext.Canvas = canvas;
            RenderKit.RenderContext.DrawElement(_view, new Rectangle(0, 0, _measured));
        }

        public override Boolean OnGenericMotionEvent(MotionEvent? e)
        {
            _gestureDetector.OnTouchEvent(e);
            return base.OnGenericMotionEvent(e);
        }

        protected override void OnMeasure(Int32 widthMeasureSpec,
                                          Int32 heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            var w = MeasuredWidth;
            var h = MeasuredHeight;

            var sz = new ValueSize(w, h);
            _measured = RenderKit.MeasureContext.MeasureMainView(_view, sz, this);
        }

        public override Boolean OnTouchEvent(MotionEvent? e)
        {
            _gestureDetector.OnTouchEvent(e);
            return base.OnTouchEvent(e);
        }

        private void OnViewPropertyChanged(Object sender,
                                           PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(IChangeTracking.IsChanged) || !_view.IsChanged)
                return;

            Invalidate();
        }

        private async Task RefreshLoop()
        {
            while (true)
                if (_view.IsChanged)
                {
                    Debug.WriteLine("view change detected");
                    _view.AcceptChanges();
                    RenderKit.MeasureContext.MeasureMainView(_view, _measured, this);
                    Invalidate();
                }
                else
                {
                    await Task.Delay(50);
                }

            // ReSharper disable once FunctionNeverReturns
        }


        public sealed override void SetOnTouchListener(IOnTouchListener? l)
        {
            base.SetOnTouchListener(l);
        }

        private readonly GestureDetectorCompat _gestureDetector;


        private readonly BaseInputHandler _inputHandler;
        private readonly Int32 _maximumFlingVelocity;
        private readonly Int32 _minimumFlingVelocity;
        private readonly IView _view;
        private ValuePoint2D? _leftButtonWentDown;

        // ReSharper disable once NotAccessedField.Local
        private Handler _loopHandler;

        private Size _measured;
    }
}