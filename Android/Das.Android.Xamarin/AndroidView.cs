using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Das.Views.Core.Geometry;
using Das.Views.Core.Input;
using Das.Views.Input;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Xamarin.Android
{
    public class AndroidView : View, IViewState, IInputContext
    {
        public AndroidView(IView view,
                           Context context,
                           IWindowManager windowManager)
            : base(context)
        {
            _measured = Size.Empty;
            _view = view;

            

            var displayMetrics = context.Resources?.DisplayMetrics ?? throw new NullReferenceException();

            var fontProvider = new AndroidFontProvider(displayMetrics);

            RenderKit = new AndroidRenderKit(new BasePerspective(), this, 
                fontProvider, windowManager);
            _inputHandler = new BaseInputHandler(RenderKit.RenderContext);

            Touch += OnTouched;
            Click += OnClick;
            Drag += OnDrag;
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


        public T GetStyleSetter<T>(StyleSetters setter, IVisualElement element)
        {
            return _view.StyleContext.GetStyleSetter<T>(setter, element);
        }

        public Double ZoomLevel => 1;

        public AndroidRenderKit RenderKit { get; }

        private MouseDownEventArgs GetArgs(MouseButtons button, MotionEvent eve)
        {
            return new MouseDownEventArgs(
                GetPosition(eve),
                button, this);
        }

        private static ValuePoint2D GetPosition(MotionEvent eve)
        {
            return new ValuePoint2D(eve.GetX(), eve.GetY());
        }

        private void OnClick(Object sender, EventArgs e)
        {
            Console.WriteLine("click");
        }

        private void OnDrag(Object sender, DragEventArgs e)
        {
            Console.WriteLine("drag " + e.Event?.GetX() + ", " + e.Event?.GetY());
        }


        protected override void OnDraw(Canvas? canvas)
        {
            if (canvas == null)
                return;

            RenderKit.RenderContext.Canvas = canvas;
            RenderKit.RenderContext.DrawElement(_view, new Rectangle(0, 0, _measured));
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

        private void OnTouched(Object sender, TouchEventArgs e)
        {
            if (!(e.Event is {} eve))
                return;

            var pos = new ValuePoint2D(eve.GetX(), eve.GetY());
            Console.WriteLine("touch # " + eve.ActionIndex + " - " + eve.Action + " " + pos);

            switch (eve.Action)
            {
                case MotionEventActions.Down:
                    _inputHandler.OnMouseDown(GetArgs(MouseButtons.Left, eve));
                    break;

                case MotionEventActions.Up:
                    _inputHandler.OnMouseUp(
                        new MouseUpEventArgs(
                            GetPosition(eve),
                            MouseButtons.Left, this));
                        
                        //GetArgs(MouseButtons.Left, eve));
                    break;
                case MotionEventActions.ButtonPress:
                    break;
                case MotionEventActions.ButtonRelease:
                    break;
                case MotionEventActions.Cancel:
                    break;
                case MotionEventActions.HoverEnter:
                    break;
                case MotionEventActions.HoverExit:
                    break;
                case MotionEventActions.HoverMove:
                    break;
                case MotionEventActions.Mask:
                    break;
                case MotionEventActions.Move:
                    break;
                case MotionEventActions.Outside:
                    break;
                case MotionEventActions.Pointer1Down:
                    break;
                case MotionEventActions.Pointer1Up:
                    break;
                case MotionEventActions.Pointer2Down:
                    break;
                case MotionEventActions.Pointer2Up:
                    break;
                case MotionEventActions.Pointer3Down:
                    break;
                case MotionEventActions.Pointer3Up:
                    break;
                case MotionEventActions.PointerIdMask:
                    break;
                case MotionEventActions.PointerIdShift:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private readonly BaseInputHandler _inputHandler;
        private readonly IView _view;
        private Size _measured;
    }
}