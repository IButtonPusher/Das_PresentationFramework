using System;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Java.Lang;
using Java.Lang.Reflect;
using Boolean = System.Boolean;
using Math = System.Math;
using Object = Java.Lang.Object;
using String = System.String;

namespace XamarinAndroidNativeTest
{
    public class ScrollViewer3 : ScrollView
    {
        public ScrollViewer3(Context? context) : base(context)
        {
            mContext = context;

            initScrollView();

            var javaScrollView = Class.FromType(typeof(ScrollView));
            var mScrollerField = javaScrollView.GetDeclaredField("mScroller");
            mScrollerField.Accessible = true;

            _mIsBeingDraggedField = javaScrollView.GetDeclaredField("mIsBeingDragged");
            _mIsBeingDraggedField.Accessible = true;

            _mVelocityTrackedField = javaScrollView.GetDeclaredField("mVelocityTracker");
            _mVelocityTrackedField.Accessible = true;


            _baseOverScroller = (OverScroller) mScrollerField.Get(this);

            var javaOverScroller = Class.FromType(typeof(OverScroller));
            //var overScrollerFields = javaOverScroller.GetDeclaredFields();
            //var overScrollerMethods = javaOverScroller.GetDeclaredMethods();

            var mScrollerYField = javaOverScroller.GetDeclaredField("mScrollerY");
            mScrollerYField.Accessible = true;
            _baseYScroller = mScrollerYField.Get(_baseOverScroller);

            var mInterpolatorField = javaOverScroller.GetDeclaredField("mInterpolator");
            mInterpolatorField.Accessible = true;
            var sumYungGuy = mInterpolatorField.Get(_baseOverScroller);
            _baseInterpolator = sumYungGuy as IInterpolator;
            

            

            var splineScrollerType = mScrollerYField.Type;


            //var maryLouRetton = javaOverScroller.GetDeclaredFields();


            _baseYVelocityField = splineScrollerType.GetDeclaredField("mCurrVelocity");
            _baseYVelocityField.Accessible = true;

            SmoothScrollingEnabled = false;
        }

        public override OverScrollMode OverScrollMode
        {
            get => base.OverScrollMode;
            set
            {
                var mode = value;
                if (mode != OverScrollMode.Never)
                {
                    if (mEdgeGlowTop == null)
                    {
                        var context = Context;
                        mEdgeGlowTop = new EdgeEffect(context);
                        mEdgeGlowBottom = new EdgeEffect(context);
                    }
                }
                else
                {
                    mEdgeGlowTop = null;
                    mEdgeGlowBottom = null;
                }

                base.OverScrollMode = value;
            }
        }

        //public override Boolean SmoothScrollingEnabled
        //{
        //    get => base.SmoothScrollingEnabled;
        //    set => base.SmoothScrollingEnabled = value;
        //}

        //public override void PostInvalidateOnAnimation()
        //{
        //    base.PostInvalidateOnAnimation();
        //}

        //public override void ScrollBy(Int32 x,
        //                              Int32 y)
        //{
        //    base.ScrollBy(x, y);
        //}

        public override void ComputeScroll()
        {
            base.ComputeScroll();
            mScroller.computeScrollOffset();
        }

        public void fling(Int32 velocityY,
                          Int32 alternativeVy)
        {
            if (ChildCount > 0)
            {
                var height = Height - PaddingBottom - PaddingTop;
                var bottom = GetChildAt(0).Height;

                //var baseV = _baseYVelocityField.GetFloat(_baseYScroller);
                //var baseV = alternativeVy;

                DumpVsces("pre-fling", alternativeVy);

              
                mScroller.fling(ScrollX, ScrollY, 0, velocityY, 0, 0, 0,
                    Math.Max(0, bottom - height), 0, height / 2);
                //if (mFlingStrictSpan == null) {
                //    mFlingStrictSpan = StrictMode.enterCriticalSpan("ScrollView-fling");
                //}
                //PostInvalidateOnAnimation();
            }
        }


        public override Boolean OnGenericMotionEvent(MotionEvent e)
        {
            if ((e.Source & InputSourceType.ClassPointer) != 0)
                switch (e.Action)
                {
                    case MotionEventActions.Scroll:
                    {
                        if (!mIsBeingDragged)
                        {
                            //var vscroll = e.GetAxisValue(Axis.Vscroll);
                            //if (vscroll != 0)
                            //{

                            //    var delta = (int) (vscroll);// * base.GetVerticalScrollFactor());
                            //    var range = getScrollRange();
                            //    int oldScrollY = ScrollY;
                            //    int newScrollY = oldScrollY - delta;
                            //    if (newScrollY < 0)
                            //    {
                            //        newScrollY = 0;
                            //    }
                            //    else if (newScrollY > range)
                            //    {
                            //        newScrollY = range;
                            //    }
                            //    if (newScrollY != oldScrollY)
                            //    {
                            //        base.ScrollTo(ScrollX, newScrollY);
                            //        return true;
                            //    }
                            //}
                        }

                        break;
                    }
                }

            return base.OnGenericMotionEvent(e);
        }

        public override Boolean OnInterceptTouchEvent(MotionEvent ev)
        {
            var action = ev.Action;
            if (action == MotionEventActions.Move && mIsBeingDragged)
                //return true;
                return base.OnInterceptTouchEvent(ev);

            switch (action & MotionEventActions.Mask)
            {
                case MotionEventActions.Move:
                {
                    var activePointerId = mActivePointerId;
                    if (activePointerId == INVALID_POINTER)
                    {
                        // If we don't have a valid id, the touch down wasn't on content.
                        break;
                    }

                    var pointerIndex = ev.FindPointerIndex(activePointerId);
                    var y = (Int32) ev.GetY(pointerIndex);
                    var yDiff = Math.Abs(y - mLastMotionY);
                    if (yDiff > mTouchSlop)
                    {
                        mIsBeingDragged = true;
                        mLastMotionY = y;
                        initVelocityTrackerIfNotExists();
                        mVelocityTracker.AddMovement(ev);
                        //if (mScrollStrictSpan == null) {
                        //    mScrollStrictSpan = StrictMode.EnterCriticalSpan("ScrollView-scroll");
                        //}
                        var parent = Parent;
                        if (parent != null)
                        {
                            //parent.RequestDisallowInterceptTouchEvent(true);
                        }
                    }

                    break;
                }
                case MotionEventActions.Down:
                {
                    var y = (Int32) ev.GetY();
                    if (!inChild((Int32) ev.GetX(), y))
                    {
                        mIsBeingDragged = false;
                        recycleVelocityTracker();
                        break;
                    }


                    mLastMotionY = y;
                    mActivePointerId = ev.GetPointerId(0);
                    initOrResetVelocityTracker();
                    mVelocityTracker.AddMovement(ev);

                    mIsBeingDragged = !mScroller.isFinished();

                    
                    //if (mIsBeingDragged && mScrollStrictSpan == null) {
                    //    mScrollStrictSpan = StrictMode.enterCriticalSpan("ScrollView-scroll");
                    //}
                    break;
                }
                case MotionEventActions.Cancel:
                case MotionEventActions.Up:
                    /* Release the drag */
                    mIsBeingDragged = false;
                    mActivePointerId = INVALID_POINTER;
                    recycleVelocityTracker();

                    if (mScroller.springBack(ScrollX, ScrollY, 0, 0, 0, getScrollRange()))
                    {
                        //PostInvalidateOnAnimation();
                    }

                    break;
                case MotionEventActions.PointerUp:

                    onSecondaryPointerUp(ev);
                    break;
            }


            var res = base.OnInterceptTouchEvent(ev);

            var ok = _mIsBeingDraggedField.GetBoolean(this);
            if (ok != mIsBeingDragged)
            {
            }
            return res;
        }

        public override Boolean OnTouchEvent(MotionEvent ev)
        {
            initVelocityTrackerIfNotExists();
            mVelocityTracker.AddMovement(ev);
            var action = ev.Action;

            var isFlingStarted = false;

            switch (action & MotionEventActions.Mask)
            {
                case MotionEventActions.Down:
                {
                    if (ChildCount == 0)
                        return false;
                    if (mIsBeingDragged = !mScroller.isFinished())
                    {
                        var parent = Parent;
                        if (parent != null)
                        {
                            //parent.RequestDisallowInterceptTouchEvent(true);
                        }
                    }


                    if (!mScroller.isFinished()) mScroller.abortAnimation();
                    //if (mFlingStrictSpan != null) {
                    //    mFlingStrictSpan.finish();
                    //    mFlingStrictSpan = null;
                    //}
                    // Remember where the motion event started
                    mLastMotionY = (Int32) ev.GetY();
                    mActivePointerId = ev.GetPointerId(0);
                    break;
                }

                case MotionEventActions.Move:
                    var activePointerIndex = ev.FindPointerIndex(mActivePointerId);
                    var y = (Int32) ev.GetY(activePointerIndex);
                    var deltaY = mLastMotionY - y;
                    if (!mIsBeingDragged && Math.Abs(deltaY) > mTouchSlop)
                    {
                        var parent = Parent;
                        if (parent != null)
                        {
                            //parent.RequestDisallowInterceptTouchEvent(true);
                        }

                        mIsBeingDragged = true;
                        if (deltaY > 0)
                            deltaY -= mTouchSlop;
                        else
                            deltaY += mTouchSlop;

                        mScroller.StartScroll(ScrollX, ScrollY, 0, deltaY);
                    }

                    if (mIsBeingDragged)
                    {
                        // Scroll to follow the motion event
                        mLastMotionY = y;
                        var oldX = ScrollX;
                        var oldY = ScrollY;
                        var range = getScrollRange();
                        var overscrollMode = OverScrollMode;
                        var canOverscroll = overscrollMode == OverScrollMode.Always ||
                                            overscrollMode == OverScrollMode.IfContentScrolls && range > 0;

                        //removed this to avoid changing base class' behavior
                        if (overScrollBy(0, deltaY, 0, ScrollY,
                            0, range, 0, mOverscrollDistance, true)) // Break our velocity if we hit a scroll barrier.
                            mVelocityTracker.Clear();

                        //removed this to avoid changing base class' behavior
                        //OnScrollChanged(ScrollX, ScrollY, oldX, oldY);
                        if (canOverscroll)
                        {
                            var pulledToY = oldY + deltaY;
                            if (pulledToY < 0)
                            {
                                mEdgeGlowTop.OnPull((Single) deltaY / Height);
                                if (!mEdgeGlowBottom.IsFinished)
                                    mEdgeGlowBottom.OnRelease();
                            }
                            else if (pulledToY > range)
                            {
                                mEdgeGlowBottom.OnPull((Single) deltaY / Height);
                                if (!mEdgeGlowTop.IsFinished)
                                    mEdgeGlowTop.OnRelease();
                            }

                            if (mEdgeGlowTop != null
                                && (!mEdgeGlowTop.IsFinished || !mEdgeGlowBottom.IsFinished))
                            {
                                //PostInvalidateOnAnimation();
                            }
                        }
                    }

                    break;
                case MotionEventActions.Up:
                    if (mIsBeingDragged)
                    {
                        var velocityTracker = mVelocityTracker;
                        velocityTracker.ComputeCurrentVelocity(1000, mMaximumVelocity);
                        var initialVelocity = (Int32) velocityTracker.GetYVelocity(mActivePointerId);

                        var otherVelocityTracker = (VelocityTracker)_mVelocityTrackedField.Get(this);
                        otherVelocityTracker.ComputeCurrentVelocity(1000, mMaximumVelocity);
                        var otherInitialVelocity = (Int32) otherVelocityTracker.GetYVelocity(mActivePointerId);

                        if (ChildCount > 0)
                        {
                            if (Math.Abs(initialVelocity) > mMinimumVelocity)
                            {
                                fling(-initialVelocity, -otherInitialVelocity);
                                isFlingStarted = true;
                            }
                            else
                            {
                                if (mScroller.springBack(ScrollX, ScrollY, 0, 0, 0, getScrollRange()))
                                {
                                    //PostInvalidateOnAnimation();
                                }
                            }
                        }

                        mActivePointerId = INVALID_POINTER;
                        endDrag();
                    }

                    break;
                case MotionEventActions.Cancel:
                    if (mIsBeingDragged && ChildCount > 0)
                    {
                        if (mScroller.springBack(ScrollX, ScrollY, 0, 0, 0, getScrollRange()))
                        {
                            //PostInvalidateOnAnimation();
                        }

                        mActivePointerId = INVALID_POINTER;
                        endDrag();
                    }

                    break;
                case MotionEventActions.PointerDown:
                {
                    var index = ev.ActionIndex;
                    mLastMotionY = (Int32) ev.GetY(index);
                    mActivePointerId = ev.GetPointerId(index);
                    break;
                }
                case MotionEventActions.PointerUp:
                    onSecondaryPointerUp(ev);
                    mLastMotionY = (Int32) ev.GetY(ev.FindPointerIndex(mActivePointerId));
                    break;

                case MotionEventActions.Scroll:
                    break;
            }

            var res = base.OnTouchEvent(ev);
            var ok = _mIsBeingDraggedField.GetBoolean(this);
            if (ok != mIsBeingDragged)
            {
            }

            if (isFlingStarted)
            {
                DumpVsces("fling started", _baseYVelocityField.GetFloat(_baseYScroller));
                //var baseV = _baseYVelocityField.GetFloat(_baseYScroller);
                //BadLog.WriteLine("After fling start " +
                //                 " SV->mScroller.V = " + baseV.ToString("0.0") + " Y: " +
                //                 _baseOverScroller.CurrY +
                //                 //" final y: " + _baseOverScroller.FinalY +
                //                 " this.mScroller.V = " + mScroller.CurrentYVelocity.ToString("0.0")
                //                 + " Y: " + mScroller.CurrY);
            }

            return res;
            //return true;
        }


        public override void RequestDisallowInterceptTouchEvent(Boolean disallowIntercept)
        {
            if (disallowIntercept)
                recycleVelocityTracker();

            base.RequestDisallowInterceptTouchEvent(disallowIntercept);
        }

        //public override void ScrollTo(Int32 x,
        //                              Int32 y)
        //{
        //    base.ScrollTo(x, y);
        //}

        protected Int32 computeHorizontalScrollExtent()
        {
            return Width;
        }

        protected Int32 computeVerticalScrollExtent()
        {
            return Height;
        }

        protected Int32 computeVerticalScrollRange()
        {
            return Height;
        }

        protected override void OnOverScrolled(Int32 scrollX,
                                               Int32 scrollY,
                                               Boolean clampedX,
                                               Boolean clampedY)
        {
            //mVelocityTracker?.Clear();

            // Treat animating scrolls differently; see #computeScroll() for why.
            if (!mScroller.isFinished())
                //mScrollX = scrollX;
                //mScrollY = scrollY;
                //invalidateParentIfNeeded();
                if (clampedY)
                    mScroller.springBack(ScrollX, ScrollY, 0, 0, 0, getScrollRange());
            //awakenScrollBars();

            //smoothScrollBy(scrollX, scrollY);

            base.OnOverScrolled(scrollX, scrollY, clampedX, clampedY);
        }


        protected override void OnScrollChanged(Int32 currentX,
                                                Int32 currentY,
                                                Int32 lastX,
                                                Int32 lastY)
        {
            base.OnScrollChanged(currentX, currentY, lastX, lastY);

            if (_lastYScrollEventValue == currentY)
                return;

            _lastYScrollEventValue = currentY;
            DumpVsces("Scrolled", _baseYVelocityField.GetFloat(_baseYScroller));
        }

        private void DumpVsces(String  why,
                               Single baseV)
        {
            //var baseV = _baseYVelocityField.GetFloat(_baseYScroller);

            BadLog.WriteLine(why + "\r\n\tbase V/S/C/E " +
                             baseV.ToString("0.0") + "/" +
                             _baseOverScroller.StartY + "/" +
                             _baseOverScroller.CurrY.ToString("0.0") + "/" +
                             _baseOverScroller.FinalY +
                             "\r\n\tthis V/S/C/E " +
                             mScroller.CurrentYVelocity.ToString("0.0") + "/" +
                             mScroller.StartY + "/" +
                             mScroller.CurrY.ToString("0.0") + "/" +
                             mScroller.FinalY);

        }

        //public override void SetSmoothScrollingEnabled(Boolean smoothScrollingEnabled)
        //{
        //}


        private Int32 computeHorizontalScrollRange()
        {
            return Width;
        }

        private void endDrag()
        {
            mIsBeingDragged = false;
            recycleVelocityTracker();
            if (mEdgeGlowTop != null)
            {
                mEdgeGlowTop.OnRelease();
                mEdgeGlowBottom.OnRelease();
            }

            //if (mScrollStrictSpan != null) {
            //    mScrollStrictSpan.finish();
            //    mScrollStrictSpan = null;
            //}
        }

        private Int32 getScrollRange()
        {
            var scrollRange = 0;
            if (ChildCount > 0)
            {
                var child = GetChildAt(0);
                scrollRange = Math.Max(0,
                    child.Height - (Height - PaddingBottom - PaddingTop));
            }

            return scrollRange;
        }

        private Boolean inChild(Int32 x,
                                Int32 y)
        {
            if (ChildCount > 0)
            {
                var scrollY = ScrollY;
                var child = GetChildAt(0);
                return !(y < child.Top - scrollY
                         || y >= child.Bottom - scrollY
                         || x < child.Left
                         || x >= child.Right);
            }

            return false;
        }


       


        private void initScrollView()
        {
            //mScroller = new OverScrollerToo(Context);
            mScroller = new OverScroller3(mContext);
            

            var configuration = ViewConfiguration.Get(mContext);
            mTouchSlop = configuration.ScaledTouchSlop;
            mMinimumVelocity = configuration.ScaledMinimumFlingVelocity;
            mMaximumVelocity = configuration.ScaledMaximumFlingVelocity;
            mOverscrollDistance = configuration.ScaledOverscrollDistance;
            mOverflingDistance = configuration.ScaledOverflingDistance;
        }

        private void initOrResetVelocityTracker()
        {
            if (mVelocityTracker == null)
                mVelocityTracker = VelocityTracker.Obtain();
            else
                mVelocityTracker.Clear();
        }

        private void initVelocityTrackerIfNotExists()
        {
            if (mVelocityTracker == null)
                mVelocityTracker = VelocityTracker.Obtain();
        }

        private void recycleVelocityTracker()
        {
            if (mVelocityTracker != null)
            {
                mVelocityTracker.Recycle();
                mVelocityTracker = null;
            }
        }

        private void onSecondaryPointerUp(MotionEvent ev)
        {
            var pointerIndex = ((Int32) ev.Action & (Int32) MotionEventActions.PointerIndexMask) >>
                               (Int32) MotionEventActions.PointerIndexShift;
            var pointerId = ev.GetPointerId(pointerIndex);
            if (pointerId == mActivePointerId)
            {
                // This was our active pointer going up. Choose a new
                // active pointer and adjust accordingly.
                // TODO: Make this decision more intelligent.
                var newPointerIndex = pointerIndex == 0 ? 1 : 0;
                mLastMotionY = (Int32) ev.GetY(newPointerIndex);
                mActivePointerId = ev.GetPointerId(newPointerIndex);
                if (mVelocityTracker != null)
                    mVelocityTracker.Clear();
            }
        }

        //protected override Boolean OverScrollBy(Int32 deltaX,
        //                                        Int32 deltaY,
        //                                        Int32 scrollX,
        //                                        Int32 scrollY,
        //                                        Int32 scrollRangeX,
        //                                        Int32 scrollRangeY,
        //                                        Int32 maxOverScrollX,
        //                                        Int32 maxOverScrollY,
        //                                        Boolean isTouchEvent)
        //{
        //    return base.OverScrollBy(deltaX, deltaY, scrollX, scrollY, scrollRangeX, scrollRangeY, maxOverScrollX, maxOverScrollY, isTouchEvent);
        //}

        private Boolean overScrollBy(Int32 deltaX,
                                     Int32 deltaY,
                                     Int32 scrollX,
                                     Int32 scrollY,
                                     Int32 scrollRangeX,
                                     Int32 scrollRangeY,
                                     Int32 maxOverScrollX,
                                     Int32 maxOverScrollY,
                                     Boolean isTouchEvent)
        {
            var overScrollMode = OverScrollMode;
            var canScrollHorizontal =
                computeHorizontalScrollRange() > computeHorizontalScrollExtent();
            var canScrollVertical =
                computeVerticalScrollRange() > computeVerticalScrollExtent();
            var overScrollHorizontal = overScrollMode == OverScrollMode.Always ||
                                       overScrollMode == OverScrollMode.IfContentScrolls
                                       && canScrollHorizontal;
            var overScrollVertical = overScrollMode == OverScrollMode.Always ||
                                     overScrollMode == OverScrollMode.IfContentScrolls && canScrollVertical;
            var newScrollX = scrollX + deltaX;
            if (!overScrollHorizontal)
                maxOverScrollX = 0;
            var newScrollY = scrollY + deltaY;
            if (!overScrollVertical)
                maxOverScrollY = 0;
            // Clamp values if at the limits and record
            var left = -maxOverScrollX;
            var right = maxOverScrollX + scrollRangeX;
            var top = -maxOverScrollY;
            var bottom = maxOverScrollY + scrollRangeY;
            var clampedX = false;
            if (newScrollX > right)
            {
                newScrollX = right;
                clampedX = true;
            }
            else if (newScrollX < left)
            {
                newScrollX = left;
                clampedX = true;
            }

            var clampedY = false;
            if (newScrollY > bottom)
            {
                newScrollY = bottom;
                clampedY = true;
            }
            else if (newScrollY < top)
            {
                newScrollY = top;
                clampedY = true;
            }

            //onOverScrolled(newScrollX, newScrollY, clampedX, clampedY);
            return clampedX || clampedY;
        }

       

        private void smoothScrollBy(Int32 dx,
                                    Int32 dy)
        {
            if (ChildCount == 0) // Nothing to do.
                return;
            var duration = AnimationUtils.CurrentAnimationTimeMillis() - mLastScroll;
            if (duration > ANIMATED_SCROLL_GAP)
            {
                var height = Height - PaddingBottom - PaddingTop;
                var bottom = GetChildAt(0).Height;
                var maxY = Math.Max(0, bottom - height);
                var scrollY = ScrollY;
                dy = Math.Max(0, Math.Min(scrollY + dy, maxY)) - scrollY;
                mScroller.StartScroll(ScrollX, scrollY, 0, dy);
                //postInvalidateOnAnimation();
            }

            mLastScroll = AnimationUtils.CurrentAnimationTimeMillis();
        }

        private const Int32 INVALID_POINTER = -1;

        private const Int32 ANIMATED_SCROLL_GAP = 250;
        private readonly Context mContext;

        private readonly IInterpolator _baseInterpolator;

        private readonly OverScroller _baseOverScroller;
        private readonly Object _baseYScroller;
        private readonly Field _baseYVelocityField;
        private Int32 _lastYScrollEventValue;
        private readonly Field _mIsBeingDraggedField;
        private readonly Field _mVelocityTrackedField;

        private Int32 mActivePointerId = INVALID_POINTER;
        private EdgeEffect mEdgeGlowBottom;

        private EdgeEffect mEdgeGlowTop;

        private Boolean mIsBeingDragged;

        private Int32 mLastMotionY;
        private Int64 mLastScroll;
        private Int32 mMaximumVelocity;
        private Int32 mMinimumVelocity;
        private Int32 mOverflingDistance;

        private Int32 mOverscrollDistance;

        private IOverScroller mScroller;

        private Int32 mTouchSlop;

        private VelocityTracker mVelocityTracker;
    }
}
