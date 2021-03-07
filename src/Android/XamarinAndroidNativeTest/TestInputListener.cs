using System;
using System.Diagnostics;
using Android.Content;
using Android.Content.Res;
using Android.Hardware;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace XamarinAndroidNativeTest
{
    public class TestInputListener: Java.Lang.Object,
                             GestureDetector.IOnGestureListener,
                             GestureDetector.IOnDoubleTapListener,
                             View.IOnTouchListener
    {
        private readonly FrameLayout _hostView;

        public TestInputListener(FrameLayout hostView,
                                 Context context,
                                 DisplayMetrics displayMetrics)
        {
            _hostView = hostView;

            _gestureDetector = new GestureDetectorCompat(context, this);
            _gestureDetector.SetOnDoubleTapListener(this);
            hostView.SetOnTouchListener(this);
            _scrollFriction = ViewConfiguration.ScrollFriction;

            _mouseWentDownAt = Stopwatch.StartNew();
            

            var ppi =  displayMetrics.Density * 160.0f;

            _physicalCoefficient = SensorManager.GravityEarth // g (m/s^2)
                                   * 39.37f // inch/meter
                                   * ppi
                                   * 0.84f; // look and feel tuning
        }

        public Boolean OnDown(MotionEvent? e)
        {
            _mouseWentDownAt = Stopwatch.StartNew();
            //return true;
            return false;
            //return _gestureDetector.OnTouchEvent(e);
        }

        public event Action? Flung;

        public Boolean OnFling(MotionEvent? e1,
                               MotionEvent? e2,
                               Single velocityX,
                               Single velocityY)
        {
            //var splineDist = GetSplineFlingDistance(Convert.ToInt32(velocityY),
            //    _scrollFriction, _physicalCoefficient);

            //var splineDur = GetSplineFlingDuration(Convert.ToInt32(velocityY),
            //    _scrollFriction, _physicalCoefficient);

            

            BadLog.WriteLine("!FLING! " + //" vx: " + velocityX + 
                             " vy: " + velocityY +
                             //" k-stopping dist: " + kineticDist.ToString("0.00") +
                             " scrollY: " + _hostView.ScrollY);
            //" spline dist: " + splineDist + " dur: " + splineDur + 
            //" host scrollY: " + _hostView.ScrollY);
            //                                 " vy2!: " + velociraptor +
            //" min: " + _minimumFlingVelocity + 
            //" e1: " + e1 + " e2: " + e2);

            //var testVelocity = 100;
            //var testDecel = -8.83;
            //var testDist = GetSplineDistance(testVelocity, testDecel);
            //var testDur = GetSplineDuration(testVelocity, testDecel);

            //for (var c = 1; c <= 10; c++)
            //{
            //    var decel = c * -600;
            //    //if (velocityY > 0)
            //    //    decel = 0 - decel;

            //    var vy = Math.Abs(velocityY) * 3.6;// * 0.277778;

            //    var friction = c * 9;
            //    var kineticDist = GetStoppingDistance(velocityY, friction);

            //    var splineDist = GetSplineDistance(vy, decel);
            //    var splineDur = GetSplineDuration(vy, decel);

            //    BadLog.WriteLine("\r\nDECELERATION: " + decel +
            //                     "\r\n------------------------\r\n\tSpline-y would have been dist: " +
            //                     splineDist.ToString("0.00") +
            //                     " duration: " + splineDur.ToString("0.00") + 
            //        "\r\n\tKinetic stopping distance: " + kineticDist + " @ friction: " + friction);
            //    //+ " or dist: " + splineDist +
            //    //" duration: " + splineDur2);
            //}

            Flung?.Invoke();

            return false;
        }

        public static Double GetSplineDuration(Double initialVelocity,
                                               Double deceleration)
        {
            var finalVelocity = 0;

            //initialVelocity *= 0.277778;

            var brakingTime = (finalVelocity - initialVelocity) / (deceleration * 3.6);
            return brakingTime;
        }

        private static Double GetStoppingDistance(Double v,
                                                  Double frictionCoeff)
        {
            var d = (v * v) / (2 * frictionCoeff * 9.8);
            return d;
        }

        public static Double GetSplineDistance(Double initialVelocity,
                                               Double deceleration)
        {
            var finalVelocity = 0;

            //initialVelocity *= 0.277778;

            var brakingDistance = (finalVelocity * finalVelocity - initialVelocity * initialVelocity)
                                  / (2 * deceleration * 3.6 * 3.6);

            return brakingDistance;
        }

        public void CalculateFling(Single velocityY)
        {
            var splineDist = GetSplineFlingDistance(Convert.ToInt32(velocityY),
                _scrollFriction, _physicalCoefficient);

            var splineDur = GetSplineFlingDuration(Convert.ToInt32(velocityY),
                _scrollFriction, _physicalCoefficient);

            BadLog.WriteLine("ScrollView FLING" + //" vx: " + velocityX + 
                             " vy: " + velocityY +
                             " spline dist: " + splineDist.ToString("0.00") + 
                             " dur: " + splineDur.ToString("0.00") + 
                             " scrollY: " + _hostView.ScrollY);
        }

        public Boolean OnTouchEvent(MotionEvent? e)
        {
            //System.Diagnostics.Debug.WriteLine("_TCH: " + e.Action);
            return _gestureDetector.OnTouchEvent(e);
        }

        public void OnLongPress(MotionEvent? e)
        {
            _gestureDetector.OnTouchEvent(e);
        }

        public Boolean OnScroll(MotionEvent? e1,
                                MotionEvent? e2,
                                Single distanceX,
                                Single distanceY)
        {
            //return true;
            return false;
        }

        public void OnShowPress(MotionEvent? e)
        {
        }

        public Boolean OnSingleTapUp(MotionEvent? e)
        {
            return false;
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

        public Boolean OnTouch(View? v,
                               MotionEvent? e)
        {
            //System.Diagnostics.Debug.WriteLine("TCH: " + e.Action);

            var res = _gestureDetector.OnTouchEvent(e);
            if (res)
            {}

            return false;
            //return true;
        }

        public void OnGenericMotionEvent(MotionEvent? e)
        {
            _gestureDetector.OnTouchEvent(e);
        }

        private Double GetSplineDeceleration(Int32 velocity,
                                             Single mFlingFriction,
                                             Single mPhysicalCoeff)
        {
            return Math.Log(_inflexion * Math.Abs(velocity) / (mFlingFriction * mPhysicalCoeff));
        }

        private Double GetSplineFlingDistance(Int32 velocity,
                                              Single mFlingFriction,
                                              Single mPhysicalCoeff)
        {
            var l = GetSplineDeceleration(velocity, mFlingFriction, mPhysicalCoeff);
            var decelMinusOne = _decelerationRate - 1.0;
            return mFlingFriction * mPhysicalCoeff * Math.Exp(_decelerationRate / decelMinusOne * l);
        }

        private Int32 GetSplineFlingDuration(Int32 velocity,
                                             Single mFlingFriction,
                                             Single mPhysicalCoeff)
        {
            var l = GetSplineDeceleration(velocity, mFlingFriction, mPhysicalCoeff);
            var decelMinusOne = _decelerationRate - 1.0;
            return (Int32) (1000.0 * Math.Exp(l / decelMinusOne));
        }

        private readonly GestureDetectorCompat _gestureDetector;

        private static readonly Single _decelerationRate = (Single) (Math.Log(0.78) / Math.Log(0.9));
        private static readonly Single _inflexion = 0.35f; // Tension lines cross at (INFLEXION, 1)

        private readonly Single _physicalCoefficient;
        private readonly Single _scrollFriction;

        private Stopwatch _mouseWentDownAt;
    }
}