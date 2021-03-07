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
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using Android.Util;

namespace XamarinAndroidNativeTest
{
    public class MainView : //ScrollView
        FrameLayout
    {
        public MainView(Context? context,
                        DisplayMetrics displayMetrics) : base(context)
        {
            

            var innerContent = new InnerView(context);

            //var sv = new ScrollViewerToo(context, displayMetrics);

            _scrollView = new ScrollView4(context);
            //_scrollView = new ScrollViewer3(context);
            _scrollView.ScrollChange += OnThisHereScrollChanged;

            _testInput = new TestInputListener(_scrollView, context, displayMetrics);
            _testInput.Flung += OnFlunged;

            //sv.GenericMotion += OnScrollGenericMotion;

            //sv.ScrollChange += OnScrollChangez;

            _scrollView.FillViewport = true;
            SetBackgroundColor(Color.Purple);
            
            var prms = new LayoutParams(1000, 1000);
            var prms2 = new LayoutParams(5000, 5000);
            var prms3 = LayoutParams.MatchParent;

            _scrollView.AddView(innerContent, prms3);
            AddView(_scrollView, prms3);

            //AddView(innerContent, prms3);

            _mouseWentDown = Stopwatch.StartNew();

            FlingChecker();
            //_testInput = new TestInputListener(this, context);
            
        }

        private void OnFlunged()
        {
            _isFlingHappening = true;
            _flingWasAt = DateTime.Now;
            _scrollWhenFlung = _scrollView.ScrollY;

        }

        private void OnThisHereScrollChanged(Object sender,
                                             ScrollChangeEventArgs e)
        {
            if (_lastScrollEventY != e.ScrollY)
            //if (_isFlingHappening)
            {
                _lastScrollChange = DateTime.Now;

                BadLog.WriteLine("scroll change: " +
                                 //_mouseWentDown.ElapsedMilliseconds + ", " + 
                                 e.ScrollY + 
                                 " => +/- " + (e.OldScrollY - e.ScrollY));
                _lastScrollEventY = e.ScrollY;
            }

            //System.Diagnostics.Debug.WriteLine("[OKYN] scroll change: " + 
            //                                   _mouseWentDown.ElapsedMilliseconds + ", " + e.ScrollY);
            
        }

        private async void FlingChecker()
        {
            while (true)
            {
                if (_isFlingHappening)
                {
                    if (_lastScroll != _scrollView.ScrollY)
                    {
                        _lastScroll = _scrollView.ScrollY;
                        await Task.Delay(10).ConfigureAwait(false);
                        continue;
                    }

                    var ts = DateTime.Now - _lastScrollChange;
                    if (ts.TotalSeconds > 0.25)
                    {
                        var totalTravelled = _scrollView.ScrollY - _scrollWhenFlung;
                        var duration = _lastScrollChange - _flingWasAt;
                        BadLog.WriteLine("*OBSERVED* Fling scroll distance: " + 
                                         totalTravelled +
                                         " duration: " + duration.TotalMilliseconds + " y is now: " + _scrollView.ScrollY);
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

        public override Boolean OnTouchEvent(MotionEvent? e)
        {
            _testInput.OnTouchEvent(e);

            if (e.Action == MotionEventActions.Up)
            {
                _mouseWentDown = Stopwatch.StartNew();
            }

            return base.OnTouchEvent(e);
        }


        public override Boolean OnGenericMotionEvent(MotionEvent? e)
        {
            _testInput.OnGenericMotionEvent(e);
            return base.OnGenericMotionEvent(e);
        }

        private ScrollView4 _scrollView;
        
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
        private Int32 _lastScrollEventY;
        
    }
}