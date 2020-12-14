using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Support.V4.View;
using Android.Views;
using Das.Extensions;
using Das.Views;
using Das.Views.Core.Geometry;
using Das.Views.Core.Input;
using Das.Views.Input;
using Das.Views.Rendering;

namespace Das.Xamarin.Android.Input
{
    public class AndroidInputContext : Java.Lang.Object,
                                       IInputContext,
                                       GestureDetector.IOnGestureListener,
                                       GestureDetector.IOnDoubleTapListener,
                                       View.IOnTouchListener
    {
        public AndroidInputContext(View hostView,
                                   Context context,
                                   BaseInputHandler inputHandler,
                                   IDisplayMetrics displayMetrics)
        {
            _hostView = hostView;
            _inputHandler = inputHandler;
            
            var viewConfig = ViewConfiguration.Get(context) ?? throw new NotSupportedException();

            _maximumFlingVelocity = viewConfig.ScaledMaximumFlingVelocity;
            _minimumFlingVelocity = viewConfig.ScaledMinimumFlingVelocity;

            
            if (displayMetrics.ZoomLevel.AreEqualEnough(1.0))
            {
                _offsetMultiplier = 0;
                _dpiRatio = 1;
                _isOffsetPositions = false;
            }
            else
            {
                _dpiRatio = 1 / displayMetrics.ZoomLevel;
                _offsetMultiplier = 1 - (1 / displayMetrics.ZoomLevel);
                _isOffsetPositions = true;
            }
            
            _gestureDetector = new GestureDetectorCompat(context, this);
            _gestureDetector.SetOnDoubleTapListener(this);

            hostView.SetOnTouchListener(this);
        }


      

        public Boolean TryCaptureMouseInput(IVisualElement view)
        {
            return _inputHandler.TryCaptureMouseInput(view);
        }

        public Boolean TryReleaseMouseCapture(IVisualElement view)
        {
            return _inputHandler.TryReleaseMouseCapture(view);
        }

        public IVisualElement? GetVisualWithMouseCapture()
        {
            return _inputHandler.GetVisualWithMouseCapture();
        }

        public Double MaximumFlingVelocity => _maximumFlingVelocity;

        public Double MinimumFlingVelocity => _minimumFlingVelocity;

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

        public void OnGenericMotionEvent(MotionEvent? e)
        {
            _gestureDetector.OnTouchEvent(e);
        }

        public void OnTouchEvent(MotionEvent? e)
        {
            _gestureDetector.OnTouchEvent(e);
        }

        public Boolean OnDown(MotionEvent? e)
        {
            if (!(e is { } eve))
                return false;

            //var pos = new ValuePoint2D(eve.GetX(), eve.GetY());
            var pos = GetPosition(eve);
            
            _leftButtonWentDown = pos;

            if (_inputHandler.OnMouseInput(new MouseDownEventArgs(
                pos, MouseButtons.Left, this), InputAction.LeftMouseButtonDown))
            {
                _hostView.Invalidate();
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
                var flingArgs = new FlingEventArgs(vx * _dpiRatio, 
                    vy * _dpiRatio, pos, this);
                _inputHandler.OnMouseInput(flingArgs, InputAction.Fling);
            }

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

            Double x = (0 - distanceX) * _dpiRatio;
            Double y = (0 - distanceY)* _dpiRatio;

            //x -= (x *_offsetMultiplier);
            //y -= (y *_offsetMultiplier);

            var delta = new ValueSize(x, y);


            var dragArgs = new DragEventArgs(start, last, delta,
                _leftButtonWentDown != null ? MouseButtons.Left : MouseButtons.Right,
                this);
            //System.Diagnostics.Debug.WriteLine("android send drag: " + dragArgs);
            _inputHandler.OnMouseInput(dragArgs, InputAction.MouseDrag);
            

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
                _hostView.Invalidate();
            return false;
        }

        void IDisposable.Dispose()
        {
            _hostView.Dispose();
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

        private ValuePoint2D GetPosition(MotionEvent eve)
        {
            if (_isOffsetPositions)
            {
                //var x = eve.GetX() - eve.GetX() * _offsetMultiplier;
                var x = eve.GetX() * _dpiRatio;
                
                //var y = eve.GetY() - eve.GetY() * _offsetMultiplier;
                var y = eve.GetY() * _dpiRatio;
                return new ValuePoint2D(x, y);
            }
            else
            {
                return new ValuePoint2D(eve.GetX(), eve.GetY());
            }
        }

        private readonly GestureDetectorCompat _gestureDetector;
        private readonly View _hostView;
        private readonly BaseInputHandler _inputHandler;
        
        private readonly Int32 _maximumFlingVelocity;
        private readonly Int32 _minimumFlingVelocity;

        private ValuePoint2D? _leftButtonWentDown;
        
        private readonly Double _offsetMultiplier;
        private readonly Double _dpiRatio;
        private readonly Boolean _isOffsetPositions;
    }
}