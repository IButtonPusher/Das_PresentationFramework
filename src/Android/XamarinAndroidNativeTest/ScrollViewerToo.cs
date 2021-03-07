using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Content.Res;
using Android.Support.V7.Widget;
using Android.Util;

namespace XamarinAndroidNativeTest
{
    public class ScrollViewerToo : ScrollView
    {
        public ScrollViewerToo(Context? context,
                               DisplayMetrics displayMetrics) : base(context)
        {
            _testInput = new TestInputListener(this, context, displayMetrics);
            _testInput.Flung += OnFlunged;

            var viewConfig = ViewConfiguration.Get(context) ?? throw new NotSupportedException();

            _maximumFlingVelocity = viewConfig.ScaledMaximumFlingVelocity;

            ScrollChange += OnThisHereScrollChanged;

            FlingChecker();
        }

        private void OnFlunged()
        {
            _isFlingHappening = true;
            _flingWasAt = DateTime.Now;
            _scrollWhenFlung = ScrollY;

        }

        private async void FlingChecker()
        {
            while (true)
            {
                if (_isFlingHappening)
                {
                    if (_lastScroll != ScrollY)
                    {
                        _lastScroll = ScrollY;
                        await Task.Delay(10).ConfigureAwait(false);
                        continue;
                    }

                    var ts = DateTime.Now - _lastScrollChange;
                    if (ts.TotalSeconds > 0.25)
                    {
                        var totalTravelled = ScrollY - _scrollWhenFlung;
                        var duration = _lastScrollChange - _flingWasAt;
                        BadLog.WriteLine("*OBSERVED* Fling scroll distance: " + 
                                                           totalTravelled +
                                                           " duration: " + duration);
                        _isFlingHappening = false;
                    }
                    else
                    {
                        await Task.Delay(10).ConfigureAwait(false);
                        continue;
                    }
                }

                await Task.Delay(100).ConfigureAwait(false);
            }
        }

        private void OnThisHereScrollChanged(Object sender,
                                             ScrollChangeEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("[OKYN] scroll change: " + 
            //                                   _mouseWentDown.ElapsedMilliseconds + ", " + e.ScrollY);
            _lastScrollChange = DateTime.Now;
        }

        public override Boolean OnTouchEvent(MotionEvent? e)
        {
            _testInput.OnTouchEvent(e);

            _velocityTracker??=VelocityTracker.Obtain();
            _velocityTracker.AddMovement(e);

            if (e.Action == MotionEventActions.Down)
            {
                _velocityTracker.Clear();
                
            }

            if (e.Action == MotionEventActions.Up)
            {
                var mActivePointerId = e.GetPointerId(0);
                //var mVelocityTracker = VelocityTracker.Obtain();

                _velocityTracker.ComputeCurrentVelocity(1000, _maximumFlingVelocity);
                var velocity = _velocityTracker.GetYVelocity(mActivePointerId);
                if (velocity != 0)
                {
                    _testInput.CalculateFling(velocity);
                }
            }

            var res = base.OnTouchEvent(e);

            return res;
        }

       


        public override Boolean OnGenericMotionEvent(MotionEvent? e)
        {
            _testInput.OnGenericMotionEvent(e);
            return base.OnGenericMotionEvent(e);
        }

        private TestInputListener _testInput;
        private Int32 _maximumFlingVelocity;
        private VelocityTracker _velocityTracker;
        private VelocityTracker mVelocityTracker;
        private Boolean mIsBeingDragged = false;
        

        private Stopwatch _mouseWentDown;
        private Boolean _isFlingHappening;
        
        private DateTime _lastScrollChange;
        private DateTime _flingWasAt;

        private Int32 _scrollWhenFlung;

        private Int32 _lastScroll;
        //private StrictMode.Span mFlingStrictSpan = null;
    }
}