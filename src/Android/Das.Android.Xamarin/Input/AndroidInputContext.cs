﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Android.Content;
using Android.Hardware;
using Android.Support.V4.View;
using Android.Views;
using Das.Extensions;
using Das.Views;
using Das.Views.Core.Geometry;
using Das.Views.Input;
using Das.Views.Rendering;


namespace Das.Xamarin.Android.Input;

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
      _activeInputActions = InputAction.None;


      //_velocityTracker = VelocityTracker.Obtain();

      var viewConfig = ViewConfiguration.Get(context) ?? throw new NotSupportedException();

      _maximumFlingVelocity = viewConfig.ScaledMaximumFlingVelocity;
      _minimumFlingVelocity = viewConfig.ScaledMinimumFlingVelocity;

      _touchSlop = viewConfig.ScaledTouchSlop;
      var ppi = displayMetrics.Density * 160.0f;


      if (displayMetrics.ZoomLevel.AreEqualEnough(1.0))
      {
         _dpiRatio = 1;
         _isOffsetPositions = false;
      }
      else
      {
         _dpiRatio = 1 / displayMetrics.ZoomLevel;
         _isOffsetPositions = true;
      }


      var _scrollFriction = ViewConfiguration.ScrollFriction;
      //_scrollFriction = (Single)(ViewConfiguration.ScrollFriction * _dpiRatio);

      var _physicalCoefficient = SensorManager.GravityEarth // g (m/s^2)
                             * 39.37f // inch/meter
                             * ppi
                             * 0.84f; // look and feel tuning


      _gestureDetector = new GestureDetectorCompat(context, this);
      _gestureDetector.SetOnDoubleTapListener(this);

      _flingBuilder = new FlingBuilder(_dpiRatio, _scrollFriction, _physicalCoefficient);

      hostView.SetOnTouchListener(this);
   }


   public Boolean TryCaptureMouseInput(IVisualElement view) => _inputHandler.TryCaptureMouseInput(view);

   public Boolean TryReleaseMouseCapture(IVisualElement view) => _inputHandler.TryReleaseMouseCapture(view);

   public IVisualElement? GetVisualWithMouseCapture() => _inputHandler.GetVisualWithMouseCapture();

   Boolean IInputContext.IsMousePresent => false;

   public Double ZoomLevel => _dpiRatio;


   public Boolean OnDoubleTap(MotionEvent? e) =>
      //System.Diagnostics.Debug.WriteLine("[OKYN] ANDROID REPORTS ON DOUBLE TAP: " + e);
      false;

   public Boolean OnDoubleTapEvent(MotionEvent? e) =>
      //System.Diagnostics.Debug.WriteLine("[OKYN] ANDROID REPORTS GENERIC DOUBLE TAP EVENT: " + e);
      false;

   public Boolean OnSingleTapConfirmed(MotionEvent? e) => false;

   public Boolean OnDown(MotionEvent? e)
   {
      //if (!(e is { } eve))
      //    return false;

      //System.Diagnostics.Debug.WriteLine("[OKYN] ANDROID REPORTS OnDown.");

      //AddInteraction(InputAction.LeftMouseButtonDown);
      //IsInteracting = true;


      var pos = GetPosition(e);
      if (pos.IsOrigin)
         return false;

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
      Debug.WriteLine("[OKYN] !FLING!: " + velocityX +
                      " vy: " + velocityY +
                      " min: " + _minimumFlingVelocity);

      if (e1 == null)
         return false;

      var pos = GetPosition(e1);

      // why 0.5 - ???
      //var vx = velocityX;// * 0.5;
      //var vy = 0 - velocityY;// * 0.5;
      velocityY = 0 - velocityY;

      if (Math.Abs(velocityX) >= _minimumFlingVelocity ||
          Math.Abs(velocityY) >= _minimumFlingVelocity)
      {
         _flingBuilder.BuildFlingValues(velocityX, out var flungX, out var xDuration);
         _flingBuilder.BuildFlingValues(velocityY, out var flungY, out var yDuration);

         var flingArgs = new FlingEventArgs(velocityX * _dpiRatio,
            velocityY * _dpiRatio, pos, this,
            flungX, flungY, xDuration, yDuration);

         _lastFlingArgs = flingArgs;

         AddInteraction(InputAction.Fling);

         if (_inputHandler.OnMouseInput(flingArgs, InputAction.Fling))
         {
            RemoveFlingEventually(flingArgs);
         }
         else
         {
            RemoveInteraction(InputAction.Fling);
         }
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
      //System.Diagnostics.Debug.WriteLine("[OKYN] ANDROID REPORTS SCROLL.");


      if (e1 == null || e2 == null)
         return false;
      

      var dragArgs = new DragEventArgs(
         GetPosition(e1), //start pos
         GetPosition(e2), //current pos
         new ValueSize( //delta
            (0 - distanceX) * _dpiRatio, //x
            (0 - distanceY) * _dpiRatio), //y,
         _leftButtonWentDown != null ? MouseButtons.Left : MouseButtons.Right,
         this);

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

      //System.Diagnostics.Debug.WriteLine("[OKYN] ANDROID REPORTS SINGLE TAP UP.");

      var pos = GetPosition(e);

      var args = new MouseUpEventArgs(
         pos, _leftButtonWentDown, MouseButtons.Left, this, true);

      if (_inputHandler.OnMouseInput(args,
             InputAction.LeftMouseButtonUp))
      {
      }

      //IsInteracting = false;
      RemoveInteraction(InputAction.LeftMouseButtonDown);
      _leftButtonWentDown = default;

      return false;
   }

   void IDisposable.Dispose()
   {
      _hostView.Dispose();
   }

   public Boolean OnTouch(View? v,
                          MotionEvent? e)
   {
      var res = _gestureDetector.OnTouchEvent(e);

      if (e == null)
         return res;

      //System.Diagnostics.Debug.WriteLine("[OKYN] ANDROID REPORTS TOUCH " + e.Action +
      //                                   " handled by gesture detector? " + res);

      switch (e.Action)
      {
         case MotionEventActions.Down:
            AddInteraction(InputAction.LeftMouseButtonDown);
            break;

         case MotionEventActions.Up:
            RemoveInteraction(InputAction.LeftMouseButtonDown);
            break;
      }

      if (res)
         return true;


      if (e?.Action != MotionEventActions.Up ||
          _leftButtonWentDown == null)
      {
         return false;
      }

      var pos = GetPosition(e);

      var distance = pos.Distance(_leftButtonWentDown);
      var willHandle = distance >= _touchSlop;

      //System.Diagnostics.Debug.WriteLine("[OKYN] ANDROID REPORTS TOUCH UP.  Distance from down:" +
      //                                   distance + " will handle: " + willHandle);

      if (!willHandle)
         return false;

      var mouseUpArgs = new MouseUpEventArgs(pos,
         _leftButtonWentDown, MouseButtons.Left, this, false);
      if (_inputHandler.OnMouseInput(mouseUpArgs,
             InputAction.LeftMouseButtonUp))
      {
      }

      RemoveInteraction(InputAction.LeftMouseButtonDown);
      //IsInteracting = false;
      SleepTime = 0;

      _leftButtonWentDown = default;

      return true;
   }

   public void OnGenericMotionEvent(MotionEvent? e)
   {
      //System.Diagnostics.Debug.WriteLine("[OKYN] ANDROID REPORTS GENERIC MOTION EVENT: " + e);

      _gestureDetector.OnTouchEvent(e);
   }

   public void OnTouchEvent(MotionEvent? e)
   {
      //System.Diagnostics.Debug.WriteLine("[OKYN] ANDROID REPORTS TOUCH EVENT: " + e);
      _gestureDetector.OnTouchEvent(e);
   }

   private void AddInteraction(InputAction action)
   {
      _activeInputActions |= action;
   }

   private void RemoveInteraction(InputAction action)
   {
      _activeInputActions &= ~action;
      if (_activeInputActions == 0)
         _activeInputActions = InputAction.None;
   }

   private async void RemoveFlingEventually(FlingEventArgs fargs)
   {
      var useTime = fargs.FlingXDuration.TotalMilliseconds >
                    fargs.FlingYDuration.TotalMilliseconds
         ? fargs.FlingXDuration
         : fargs.FlingYDuration;

      await Task.Delay(useTime).ConfigureAwait(false);

      if (!Equals(_lastFlingArgs, fargs))
         return;

      RemoveInteraction(InputAction.Fling);
   }

   private ValuePoint2D GetPosition(MotionEvent? eve)
   {
      if (ReferenceEquals(null, eve))
         return ValuePoint2D.Empty;

      if (_isOffsetPositions)
      {
         var x = eve.GetX() * _dpiRatio;
         var y = eve.GetY() * _dpiRatio;
         return new ValuePoint2D(x, y);
      }

      return new ValuePoint2D(eve.GetX(), eve.GetY());
   }

   //public GestureDetectorCompat GestureDetector => _gestureDetector;

   public Boolean IsInteracting => _activeInputActions != InputAction.None;

   private readonly Double _dpiRatio;
   private readonly FlingBuilder _flingBuilder;

   private readonly GestureDetectorCompat _gestureDetector;
   private readonly View _hostView;
   private readonly BaseInputHandler _inputHandler;
   private readonly Boolean _isOffsetPositions;

   // ReSharper disable once NotAccessedField.Local
   private readonly Int32 _maximumFlingVelocity;
   private readonly Int32 _minimumFlingVelocity;
   //private readonly Single _physicalCoefficient;
   //private readonly Single _scrollFriction;
   private readonly Double _touchSlop;

   private InputAction _activeInputActions;
   private FlingEventArgs? _lastFlingArgs;

   private ValuePoint2D? _leftButtonWentDown;

   public Int32 SleepTime;

   //private static readonly Single _decelerationRate = (Single) (Math.Log(0.78) / Math.Log(0.9));
   //private static readonly Single _inflexion = 0.35f; // Tension lines cross at (INFLEXION, 1)
}
