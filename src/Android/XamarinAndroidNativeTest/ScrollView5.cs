using System;
using System.Collections.Generic;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Accessibility;
using Android.Widget;
using Java.Lang;
using XamarinAndroidNativeTest;
using Boolean = System.Boolean;
using String = System.String;

//using Java.Lang;

/**
 * A view group that allows the view hierarchy placed within it to be scrolled.
 * Scroll view may have only one direct child placed within it.
 * To add multiple views within the scroll view, make
 * the direct child you add a view group, for example {@link LinearLayout}, and
 * place additional views within that LinearLayout.
 *
 * <p>Scroll view supports vertical scrolling only. For horizontal scrolling,
 * use {@link HorizontalScrollView} instead.</p>
 *
 * <p>Never add a {@link android.support.v7.widget.RecyclerView} or {@link ListView} to
 * a scroll view. Doing so results in poor user interface performance and a poor user
 * experience.</p>
 *
 * <p class="note">
 * For vertical scrolling, consider {@link android.support.v4.widget.NestedScrollView}
 * instead of scroll view which offers greater user interface flexibility and
 * support for the material design scrolling patterns.</p>
 *
 * <p>To learn more about material design patterns for handling scrolling, see
 * <a href="https://material.io/guidelines/patterns/scrolling-techniques.html#">
 * Scrolling techniques</a>.</p>
 *
 * @attr ref android.R.styleable#ScrollView_fillViewport
 */
public class ScrollView5 : FrameLayout {
    const int ANIMATED_SCROLL_GAP = 250;
    const float MAX_SCROLL_FACTOR = 0.5f;
    private const String TAG = "ScrollView";
    //@UnsupportedAppUsage
    private long mLastScroll;
    private readonly Rect mTempRect = new Rect();
    //@UnsupportedAppUsage
    private OverScroller mScroller;
    /**
     * Tracks the state of the top edge glow.
     *
     * Even though this field is practically final, we cannot make it final because there are apps
     * setting it via reflection and they need to keep working until they target Q.
     */
    //@NonNull
    //@UnsupportedAppUsage(maxTargetSdk = Build.VERSION_CODES.P, trackingBug = 123768600)
    private EdgeEffect mEdgeGlowTop;// = new EdgeEffect(Context);
    /**
     * Tracks the state of the bottom edge glow.
     *
     * Even though this field is practically final, we cannot make it final because there are apps
     * setting it via reflection and they need to keep working until they target Q.
     */
    //@NonNull
    //@UnsupportedAppUsage(maxTargetSdk = Build.VERSION_CODES.P, trackingBug = 123769386)
    private EdgeEffect mEdgeGlowBottom;// = new EdgeEffect(Context);
    /**
     * Position of the last motion e.
     */
    //@UnsupportedAppUsage
    private int mLastMotionY;
    /**
     * True when the layout has changed but the traversal has not come through yet.
     * Ideally the view hierarchy would keep track of this for us.
     */
    private Boolean mIsLayoutDirty = true;
    /**
     * The child to give focus to in the e that a child has requested focus while the
     * layout is dirty. This prevents the scroll from being wrong if the child has not been
     * laid out before requesting focus.
     */
    //@UnsupportedAppUsage(maxTargetSdk = Build.VERSION_CODES.P, trackingBug = 123769715)
    private View mChildToScrollTo = null;
    /**
     * True if the user is currently dragging this ScrollView around. This is
     * not the same as 'is being flinged', which can be checked by
     * mScroller.isFinished() (flinging begins when the user lifts his finger).
     */
    //@UnsupportedAppUsage
    private Boolean mIsBeingDragged = false;
    /**
     * Determines speed during touch scrolling
     */
    //@UnsupportedAppUsage
    private VelocityTracker mVelocityTracker;
    /**
     * When set to true, the scroll view measure its child to make it fill the currently
     * visible area.
     */
    //@ViewDebug.ExportedProperty(category = "layout")
    private Boolean mFillViewport;
    /**
     * Whether arrow scrolling is animated.
     */
    private Boolean mSmoothScrollingEnabled = true;
    private int mTouchSlop;
    //@UnsupportedAppUsage(maxTargetSdk = Build.VERSION_CODES.P, trackingBug = 124051125)
    private int mMinimumVelocity;
    private int mMaximumVelocity;
    //@UnsupportedAppUsage(maxTargetSdk = Build.VERSION_CODES.P, trackingBug = 124050903)
    private int mOverscrollDistance;
    //@UnsupportedAppUsage(maxTargetSdk = Build.VERSION_CODES.P, trackingBug = 124050903)
    private int mOverflingDistance;
    private float mVerticalScrollFactor;
    /**
     * ID of the active pointer. This is used to retain consistency during
     * drags/flings if multiple pointers are used.
     */
    private int mActivePointerId = INVALID_POINTER;
    /**
     * Used during scrolling to retrieve the new offset within the window.
     */
    private readonly int[] mScrollOffset = new int[2];
    private readonly int[] mScrollConsumed = new int[2];
    private int mNestedYOffset;
    /**
     * The StrictMode "critical time span" objects to catch animation
     * stutters.  Non-null when a time-sensitive animation is
     * in-flight.  Must call finish() on them when done animating.
     * These are no-ops on user builds.
     */
    private StrictMode2.Span mScrollStrictSpan = null;  // aka "drag"
    //@UnsupportedAppUsage
    private StrictMode2.Span mFlingStrictSpan = null;
    /**
     * Sentinel value for no current active pointer.
     * Used by {@link #mActivePointerId}.
     */
    private const int INVALID_POINTER = -1;
    private SavedState mSavedState;
    public ScrollView5(Context context) : this(context, null){
        
    }
    public ScrollView5(Context context, IAttributeSet attrs)
    : this(context, attrs, ScrollViewStyle)
    {
        
    }

    public const int ScrollViewStyle = 16842880;

    public ScrollView5(Context context, IAttributeSet attrs, int defStyleAttr)
    : this(context, attrs, defStyleAttr, 0)
    {

    }


    public ScrollView5(Context context,
                      IAttributeSet attrs,
                      int defStyleAttr,
                      int defStyleRes)
        : base(context, attrs, defStyleAttr, defStyleRes)
    {

        initScrollView();
        Android.Content.Res.TypedArray a = context.ObtainStyledAttributes(attrs,
            new Int32[]
            {
                unchecked((int) (0x0102023c)),
                defStyleRes, 0
            });
        //var a = context.ObtainStyledAttributes(
        //        attrs, com.android.)internal.R.styleable.ScrollView, defStyleAttr, defStyleRes);
        //    saveAttributeDataForStyleable(context, com.android.internal.R.styleable.ScrollView,
        //            attrs, a, defStyleAttr, defStyleRes);

        IsFillViewport = a.GetBoolean(0, false);
        
        a.Recycle();
        if (context.Resources.Configuration.UiMode == UiMode.TypeWatch)
        {
            RevealOnFocusHint = false;

        }
    }

    public override Boolean ShouldDelayChildPressedState()
    {
        return true;
    }

    protected override Single TopFadingEdgeStrength
    {
        get
        {
            if (ChildCount == 0)
            {
                return 0.0f;
            }

            var length = VerticalFadingEdgeLength;
            if (ScrollY < length)
            {
                return ScrollY / (float) length;
            }

            return 1.0f;
        }
    }

    //@Override
    protected override float BottomFadingEdgeStrength
    {
        get
        {
            if (ChildCount == 0)
            {
                return 0.0f;
            }

            var length = VerticalFadingEdgeLength;
            var bottomEdge = Height - PaddingBottom;
            var span = GetChildAt(0).Bottom - ScrollY - bottomEdge;
            if (span < length)
            {
                return span / (float) length;
            }

            return 1.0f;
        }
    }

    /**
     * Sets the edge effect color for both top and bottom edge effects.
     *
     * @param color The color for the edge effects.
     * @see #setTopEdgeEffectColor(int)
     * @see #setBottomEdgeEffectColor(int)
     * @see #getTopEdgeEffectColor()
     * @see #getBottomEdgeEffectColor()
     */
    public void setEdgeEffectColor(Color color)
    {
        setTopEdgeEffectColor(color);
        setBottomEdgeEffectColor(color);
    }
    /**
     * Sets the bottom edge effect color.
     *
     * @param color The color for the bottom edge effect.
     * @see #setTopEdgeEffectColor(int)
     * @see #setEdgeEffectColor(int)
     * @see #getTopEdgeEffectColor()
     * @see #getBottomEdgeEffectColor()
     */
    public void setBottomEdgeEffectColor(Color color)
    {
        mEdgeGlowBottom.SetColor(color);
    }
    /**
     * Sets the top edge effect color.
     *
     * @param color The color for the top edge effect.
     * @see #setBottomEdgeEffectColor(int)
     * @see #setEdgeEffectColor(int)
     * @see #getTopEdgeEffectColor()
     * @see #getBottomEdgeEffectColor()
     */
    public void setTopEdgeEffectColor(Color color)
    {
        mEdgeGlowTop.SetColor(color);
    }
    /**
     * Returns the top edge effect color.
     *
     * @return The top edge effect color.
     * @see #setEdgeEffectColor(int)
     * @see #setTopEdgeEffectColor(int)
     * @see #setBottomEdgeEffectColor(int)
     * @see #getBottomEdgeEffectColor()
     */
    
        public int getTopEdgeEffectColor()
    {
        return mEdgeGlowTop.Color;
    }
    /**
     * Returns the bottom edge effect color.
     *
     * @return The bottom edge effect color.
     * @see #setEdgeEffectColor(int)
     * @see #setTopEdgeEffectColor(int)
     * @see #setBottomEdgeEffectColor(int)
     * @see #getTopEdgeEffectColor()
     */
    
        public int getBottomEdgeEffectColor()
    {
        return mEdgeGlowBottom.Color;
    }
    /**
     * @return The maximum amount this scroll view will scroll in response to
     *   an arrow e.
     */
    public int getMaxScrollAmount()
    {
        return (int)(MAX_SCROLL_FACTOR * (Bottom - Top));
    }
    private void initScrollView()
    {
        mScroller = new OverScroller(Context);
        Focusable = true;

        DescendantFocusability = DescendantFocusability.AfterDescendants;
        SetWillNotDraw(false);
        var configuration = ViewConfiguration.Get(Context);
        mTouchSlop = configuration.ScaledTouchSlop;
        mMinimumVelocity = configuration.ScaledMinimumFlingVelocity;
        mMaximumVelocity = configuration.ScaledMaximumFlingVelocity;
        mOverscrollDistance = configuration.ScaledOverscrollDistance;
        mOverflingDistance = configuration.ScaledOverflingDistance;
        mVerticalScrollFactor = configuration.ScaledVerticalScrollFactor;
    }
    //@Override
    public override void AddView(View child)
    {
        if (ChildCount > 0)
        {
            throw new IllegalStateException("ScrollView can host only one direct child");
        }
        base.AddView(child);
    }
    //@Override
    public override void AddView(View child, int index)
    {
        if (ChildCount > 0)
        {
            throw new IllegalStateException("ScrollView can host only one direct child");
        }
        base.AddView(child, index);
    }
    //@Override
    public override  void AddView(View child, ViewGroup.LayoutParams p)
    {
        if (ChildCount > 0)
        {
            throw new IllegalStateException("ScrollView can host only one direct child");
        }
        base.AddView(child, p);
    }
    //@Override
    public override void AddView(View child, int index, ViewGroup.LayoutParams p)
    {
        if (ChildCount > 0)
        {
            throw new IllegalStateException("ScrollView can host only one direct child");
        }
        base.AddView(child, index, p);
    }
    /**
     * @return Returns true this ScrollView can be scrolled
     */
    //@UnsupportedAppUsage
    private Boolean canScroll()
    {
        View child = GetChildAt(0);
        if (child != null)
        {
            int childHeight = child.Height;
            return Height < childHeight + PaddingTop + PaddingBottom;
        }
        return false;
    }
    /**
     * Indicates whether this ScrollView's content is stretched to fill the viewport.
     *
     * @return True if the content fills the viewport, false otherwise.
     *
     * @attr ref android.R.styleable#ScrollView_fillViewport
     */

    public Boolean IsFillViewport
    {
        get => mFillViewport;
        set
        {
            if (value != mFillViewport) {
                mFillViewport = value;
                RequestLayout();
            }
        }
    }

    public Boolean IsSmoothScrollEnabled
    {
        get => mSmoothScrollingEnabled;
        set => mSmoothScrollingEnabled = value;
        
    }



    //@Override
    protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
    {
        base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
        if (!mFillViewport)
        {
            return;
        }
        var heightMode = View.MeasureSpec.GetMode(heightMeasureSpec);
        if (heightMode == MeasureSpecMode.Unspecified)
        {
            return;
        }
        if (ChildCount > 0)
        {
            var child = GetChildAt(0);
            Int32 widthPadding;
            Int32 heightPadding;
            var targetSdkVersion = Context.ApplicationInfo.TargetSdkVersion;
            var lp = (LayoutParams)child.LayoutParameters;
            if (targetSdkVersion >= BuildVersionCodes.M)
            {
                widthPadding = PaddingLeft + PaddingRight + lp.LeftMargin + lp.RightMargin;
                heightPadding = PaddingTop + PaddingBottom + lp.TopMargin + lp.BottomMargin;
            }
            else
            {
                widthPadding = PaddingLeft + PaddingRight;
                heightPadding = PaddingTop + PaddingBottom;
            }
            var desiredHeight = MeasuredHeight - heightPadding;
            if (child.MeasuredHeight < desiredHeight)
            {
                var  childWidthMeasureSpec = GetChildMeasureSpec(
                        widthMeasureSpec, widthPadding, lp.Width);
                var  childHeightMeasureSpec = View.MeasureSpec.MakeMeasureSpec(
                        desiredHeight, MeasureSpecMode.Exactly);
                child.Measure(childWidthMeasureSpec, childHeightMeasureSpec);
            }
        }
    }
    //@Override
    public override Boolean DispatchKeyEvent(KeyEvent e) {
        // Let the focused view and/or our descendants get the key first
        return base.DispatchKeyEvent(e) || executeKeyEvent(e);
    }
    /**
     * You can call this function yourself to have the scroll view perform
     * scrolling from a key e, just as if the e had been dispatched to
     * it by the view hierarchy.
     *
     * @param e The key e to execute.
     * @return Return true if the e was handled, else false.
     */
    public Boolean executeKeyEvent(KeyEvent e) {
        mTempRect.SetEmpty();
        if (!canScroll())
        {
            if (IsFocused && e.KeyCode != Keycode.Back) {
                    View currentFocused = FindFocus();
                    if (currentFocused == this) currentFocused = null;
                    View nextFocused = FocusFinder.Instance.FindNextFocus(this,
                            currentFocused, FocusSearchDirection.Down);
                    return nextFocused != null
                            && nextFocused != this
                            && nextFocused.RequestFocus(FocusSearchDirection.Down);
                }
return false;
            }
            Boolean handled = false;
if (e.Action ==  KeyEventActions.Down) {
    switch (e.KeyCode) {
                    case  Keycode.DpadUp:
                        if (!e.IsAltPressed) {
            handled = arrowScroll(FOCUS_UP);
        } else
        {
            handled = fullScroll(FOCUS_UP);
        }
        break;
                    case Keycode.DpadDown:
                        if (!e.IsAltPressed) {
            handled = arrowScroll(FOCUS_DOWN);
        } else
        {
            handled = fullScroll(FOCUS_DOWN);
        }
        break;
                    case Keycode.Space:
                        pageScroll(e.IsShiftPressed ? FOCUS_UP : FOCUS_DOWN);
        break;
    }
}
            return handled;
}

    public const int FOCUS_BACKWARD = 0x00000001;
    /**
     * Use with {@link #focusSearch(int)}. Move focus to the next selectable
     * item.
     */
    public const int FOCUS_FORWARD = 0x00000002;
    /**
     * Use with {@link #focusSearch(int)}. Move focus to the left.
     */
    public const int FOCUS_LEFT = 0x00000011;
    /**
     * Use with {@link #focusSearch(int)}. Move focus up.
     */
    public const int FOCUS_UP = 0x00000021;
    /**
     * Use with {@link #focusSearch(int)}. Move focus to the right.
     */
    public const int FOCUS_RIGHT = 0x00000042;
    /**
     * Use with {@link #focusSearch(int)}. Move focus down.
     */
    public const int FOCUS_DOWN = 0x00000082;

    //    private Boolean inChild(int x, int y) {
    //        if (ChildCount > 0) {
    //            final int scrollY = mScrollY;
    //            final View child = GetChildAt(0);
    //            return !(y < child.getTop() - scrollY
    //                    || y >= child.Bottom - scrollY
    //                    || x < child.getLeft()
    //                    || x >= child.getRight());
    //        }
    //        return false;
    //    }
    //    private void initOrResetVelocityTracker() {
    //        if (mVelocityTracker == null) {
    //            mVelocityTracker = VelocityTracker.obtain();
    //        } else {
    //            mVelocityTracker.clear();
    //        }
    //    }
    //    private void initVelocityTrackerIfNotExists() {
    //        if (mVelocityTracker == null) {
    //            mVelocityTracker = VelocityTracker.obtain();
    //        }
    //    }
    //    private void recycleVelocityTracker() {
    //        if (mVelocityTracker != null) {
    //            mVelocityTracker.recycle();
    //            mVelocityTracker = null;
    //        }
    //    }
    //    //@Override
    //    public void requestDisallowInterceptTouchEvent(Boolean disallowIntercept) {
    //        if (disallowIntercept) {
    //            recycleVelocityTracker();
    //        }
    //        base.requestDisallowInterceptTouchEvent(disallowIntercept);
    //    }
    //    //@Override
    //    public Boolean onInterceptTouchEvent(MotionEvent ev) {
    //        /*
    //         * This method JUST determines whether we want to intercept the motion.
    //         * If we return true, onMotionEvent will be called and we do the actual
    //         * scrolling there.
    //         */
    //        /*
    //        * Shortcut the most recurring case: the user is in the dragging
    //        * state and he is moving his finger.  We want to intercept this
    //        * motion.
    //        */
    //        final int action = ev.getAction();
    //        if ((action == MotionEvent.ACTION_MOVE) && (mIsBeingDragged)) {
    //            return true;
    //        }
    //        if (base.onInterceptTouchEvent(ev)) {
    //            return true;
    //        }
    //        /*
    //         * Don't try to intercept touch if we can't scroll anyway.
    //         */
    //        if (ScrollY == 0 && !canScrollVertically(1)) {
    //            return false;
    //        }
    //        switch (action & MotionEvent.ACTION_MASK) {
    //            case MotionEvent.ACTION_MOVE: {
    //                /*
    //                 * mIsBeingDragged == false, otherwise the shortcut would have caught it. Check
    //                 * whether the user has moved far enough from his original down touch.
    //                 */
    //                /*
    //                * Locally do absolute value. mLastMotionY is set to the y value
    //                * of the down e.
    //                */
    //                final int activePointerId = mActivePointerId;
    //                if (activePointerId == INVALID_POINTER) {
    //                    // If we don't have a valid id, the touch down wasn't on content.
    //                    break;
    //                }
    //                final int pointerIndex = ev.findPointerIndex(activePointerId);
    //                if (pointerIndex == -1) {
    //                    Log.e(TAG, "Invalid pointerId=" + activePointerId
    //                            + " in onInterceptTouchEvent");
    //                    break;
    //                }
    //                final int y = (int) ev.getY(pointerIndex);
    //                final int yDiff = Math.abs(y - mLastMotionY);
    //                if (yDiff > mTouchSlop && (getNestedScrollAxes() & SCROLL_AXIS_VERTICAL) == 0) {
    //                    mIsBeingDragged = true;
    //                    mLastMotionY = y;
    //                    initVelocityTrackerIfNotExists();
    //                    mVelocityTracker.addMovement(ev);
    //                    mNestedYOffset = 0;
    //                    if (mScrollStrictSpan == null) {
    //                        mScrollStrictSpan = StrictMode.enterCriticalSpan("ScrollView-scroll");
    //                    }
    //                    final ViewParent parent = getParent();
    //                    if (parent != null) {
    //                        parent.requestDisallowInterceptTouchEvent(true);
    //                    }
    //                }
    //                break;
    //            }
    //            case MotionEvent.ACTION_DOWN: {
    //                final int y = (int) ev.getY();
    //                if (!inChild((int) ev.getX(), (int) y)) {
    //                    mIsBeingDragged = false;
    //                    recycleVelocityTracker();
    //                    break;
    //                }
    //                /*
    //                 * Remember location of down touch.
    //                 * ACTION_DOWN always refers to pointer index 0.
    //                 */
    //                mLastMotionY = y;
    //                mActivePointerId = ev.getPointerId(0);
    //                initOrResetVelocityTracker();
    //                mVelocityTracker.addMovement(ev);
    //                /*
    //                 * If being flinged and user touches the screen, initiate drag;
    //                 * otherwise don't. mScroller.isFinished should be false when
    //                 * being flinged. We need to call computeScrollOffset() first so that
    //                 * isFinished() is correct.
    //                */
    //                mScroller.computeScrollOffset();
    //                mIsBeingDragged = !mScroller.isFinished();
    //                if (mIsBeingDragged && mScrollStrictSpan == null) {
    //                    mScrollStrictSpan = StrictMode.enterCriticalSpan("ScrollView-scroll");
    //                }
    //                startNestedScroll(SCROLL_AXIS_VERTICAL);
    //                break;
    //            }
    //            case MotionEvent.ACTION_CANCEL:
    //            case MotionEvent.ACTION_UP:
    //                /* Release the drag */
    //                mIsBeingDragged = false;
    //                mActivePointerId = INVALID_POINTER;
    //                recycleVelocityTracker();
    //                if (mScroller.springBack(mScrollX, mScrollY, 0, 0, 0, getScrollRange())) {
    //                    postInvalidateOnAnimation();
    //                }
    //                stopNestedScroll();
    //                break;
    //            case MotionEvent.ACTION_POINTER_UP:
    //                onSecondaryPointerUp(ev);
    //                break;
    //        }
    //        /*
    //        * The only time we want to intercept motion events is if we are in the
    //        * drag mode.
    //        */
    //        return mIsBeingDragged;
    //    }
    //    private Boolean shouldDisplayEdgeEffects() {
    //        return getOverScrollMode() != OVER_SCROLL_NEVER;
    //    }
    //    //@Override
    //    public Boolean onTouchEvent(MotionEvent ev) {
    //        initVelocityTrackerIfNotExists();
    //        MotionEvent vtev = MotionEvent.obtain(ev);
    //        final int actionMasked = ev.getActionMasked();
    //        if (actionMasked == MotionEvent.ACTION_DOWN) {
    //            mNestedYOffset = 0;
    //        }
    //        vtev.offsetLocation(0, mNestedYOffset);
    //        switch (actionMasked) {
    //            case MotionEvent.ACTION_DOWN: {
    //                if (ChildCount == 0) {
    //                    return false;
    //                }
    //                if ((mIsBeingDragged = !mScroller.isFinished())) {
    //                    final ViewParent parent = getParent();
    //                    if (parent != null) {
    //                        parent.requestDisallowInterceptTouchEvent(true);
    //                    }
    //                }
    //                /*
    //                 * If being flinged and user touches, stop the fling. isFinished
    //                 * will be false if being flinged.
    //                 */
    //                if (!mScroller.isFinished()) {
    //                    mScroller.abortAnimation();
    //                    if (mFlingStrictSpan != null) {
    //                        mFlingStrictSpan.finish();
    //                        mFlingStrictSpan = null;
    //                    }
    //                }
    //                // Remember where the motion e started
    //                mLastMotionY = (int) ev.getY();
    //                mActivePointerId = ev.getPointerId(0);
    //                startNestedScroll(SCROLL_AXIS_VERTICAL);
    //                break;
    //            }
    //            case MotionEvent.ACTION_MOVE:
    //                final int activePointerIndex = ev.findPointerIndex(mActivePointerId);
    //                if (activePointerIndex == -1) {
    //                    Log.e(TAG, "Invalid pointerId=" + mActivePointerId + " in onTouchEvent");
    //                    break;
    //                }
    //                final int y = (int) ev.getY(activePointerIndex);
    //                int deltaY = mLastMotionY - y;
    //                if (dispatchNestedPreScroll(0, deltaY, mScrollConsumed, mScrollOffset)) {
    //                    deltaY -= mScrollConsumed[1];
    //                    vtev.offsetLocation(0, mScrollOffset[1]);
    //                    mNestedYOffset += mScrollOffset[1];
    //                }
    //                if (!mIsBeingDragged && Math.abs(deltaY) > mTouchSlop) {
    //                    final ViewParent parent = getParent();
    //                    if (parent != null) {
    //                        parent.requestDisallowInterceptTouchEvent(true);
    //                    }
    //                    mIsBeingDragged = true;
    //                    if (deltaY > 0) {
    //                        deltaY -= mTouchSlop;
    //                    } else {
    //                        deltaY += mTouchSlop;
    //                    }
    //                }
    //                if (mIsBeingDragged) {
    //                    // Scroll to follow the motion e
    //                    mLastMotionY = y - mScrollOffset[1];
    //                    final int oldY = mScrollY;
    //                    final int range = getScrollRange();
    //                    final int overscrollMode = getOverScrollMode();
    //                    Boolean canOverscroll = overscrollMode == OVER_SCROLL_ALWAYS ||
    //                            (overscrollMode == OVER_SCROLL_IF_CONTENT_SCROLLS && range > 0);
    //                    // Calling overScrollBy will call onOverScrolled, which
    //                    // calls onScrollChanged if applicable.
    //                    if (overScrollBy(0, deltaY, 0, mScrollY, 0, range, 0, mOverscrollDistance, true)
    //                            && !hasNestedScrollingParent()) {
    //                        // Break our velocity if we hit a scroll barrier.
    //                        mVelocityTracker.clear();
    //                    }
    //                    final int scrolledDeltaY = mScrollY - oldY;
    //                    final int unconsumedY = deltaY - scrolledDeltaY;
    //                    if (dispatchNestedScroll(0, scrolledDeltaY, 0, unconsumedY, mScrollOffset)) {
    //                        mLastMotionY -= mScrollOffset[1];
    //                        vtev.offsetLocation(0, mScrollOffset[1]);
    //                        mNestedYOffset += mScrollOffset[1];
    //                    } else if (canOverscroll) {
    //                        final int pulledToY = oldY + deltaY;
    //                        if (pulledToY < 0) {
    //                            mEdgeGlowTop.onPull((float) deltaY / Height,
    //                                    ev.getX(activePointerIndex) / getWidth());
    //                            if (!mEdgeGlowBottom.isFinished()) {
    //                                mEdgeGlowBottom.onRelease();
    //                            }
    //                        } else if (pulledToY > range) {
    //                            mEdgeGlowBottom.onPull((float) deltaY / Height,
    //                                    1.` - ev.getX(activePointerIndex) / getWidth());
    //                            if (!mEdgeGlowTop.isFinished()) {
    //                                mEdgeGlowTop.onRelease();
    //                            }
    //                        }
    //                        if (shouldDisplayEdgeEffects()
    //                                && (!mEdgeGlowTop.isFinished() || !mEdgeGlowBottom.isFinished())) {
    //                            postInvalidateOnAnimation();
    //                        }
    //                    }
    //                }
    //                break;
    //            case MotionEvent.ACTION_UP:
    //                if (mIsBeingDragged) {
    //                    final VelocityTracker velocityTracker = mVelocityTracker;
    //                    velocityTracker.computeCurrentVelocity(1000, mMaximumVelocity);
    //                    int initialVelocity = (int) velocityTracker.getYVelocity(mActivePointerId);
    //                    if ((Math.abs(initialVelocity) > mMinimumVelocity)) {
    //                        flingWithNestedDispatch(-initialVelocity);
    //                    } else if (mScroller.springBack(mScrollX, mScrollY, 0, 0, 0,
    //                            getScrollRange())) {
    //                        postInvalidateOnAnimation();
    //                    }
    //                    mActivePointerId = INVALID_POINTER;
    //                    endDrag();
    //                }
    //                break;
    //            case MotionEvent.ACTION_CANCEL:
    //                if (mIsBeingDragged && ChildCount > 0) {
    //                    if (mScroller.springBack(mScrollX, mScrollY, 0, 0, 0, getScrollRange())) {
    //                        postInvalidateOnAnimation();
    //                    }
    //                    mActivePointerId = INVALID_POINTER;
    //                    endDrag();
    //                }
    //                break;
    //            case MotionEvent.ACTION_POINTER_DOWN: {
    //                final int index = ev.getActionIndex();
    //                mLastMotionY = (int) ev.getY(index);
    //                mActivePointerId = ev.getPointerId(index);
    //                break;
    //            }
    //            case MotionEvent.ACTION_POINTER_UP:
    //                onSecondaryPointerUp(ev);
    //                mLastMotionY = (int) ev.getY(ev.findPointerIndex(mActivePointerId));
    //                break;
    //        }
    //        if (mVelocityTracker != null) {
    //            mVelocityTracker.addMovement(vtev);
    //        }
    //        vtev.recycle();
    //        return true;
    //    }
    //    private void onSecondaryPointerUp(MotionEvent ev) {
    //        final int pointerIndex = (ev.getAction() & MotionEvent.ACTION_POINTER_INDEX_MASK) >>
    //                MotionEvent.ACTION_POINTER_INDEX_SHIFT;
    //        final int pointerId = ev.getPointerId(pointerIndex);
    //        if (pointerId == mActivePointerId) {
    //            // This was our active pointer going up. Choose a new
    //            // active pointer and adjust accordingly.
    //            // TODO: Make this decision more intelligent.
    //            final int newPointerIndex = pointerIndex == 0 ? 1 : 0;
    //            mLastMotionY = (int) ev.getY(newPointerIndex);
    //            mActivePointerId = ev.getPointerId(newPointerIndex);
    //            if (mVelocityTracker != null) {
    //                mVelocityTracker.clear();
    //            }
    //        }
    //    }
    //    //@Override
    //    public Boolean onGenericMotionEvent(MotionEvent e) {
    //        switch (e.getAction()) {
    //            case MotionEvent.ACTION_SCROLL:
    //                final float axisValue;
    //                if (e.isFromSource(InputDevice.SOURCE_CLASS_POINTER)) {
    //                    axisValue = e.getAxisValue(MotionEvent.AXIS_VSCROLL);
    //                } else if (e.isFromSource(InputDevice.SOURCE_ROTARY_ENCODER)) {
    //                    axisValue = e.getAxisValue(MotionEvent.AXIS_SCROLL);
    //                } else {
    //                    axisValue = 0;
    //                }
    //                final int delta = Math.round(axisValue * mVerticalScrollFactor);
    //                if (delta != 0) {
    //                    final int range = getScrollRange();
    //                    int oldScrollY = mScrollY;
    //                    int newScrollY = oldScrollY - delta;
    //                    if (newScrollY < 0) {
    //                        newScrollY = 0;
    //                    } else if (newScrollY > range) {
    //                        newScrollY = range;
    //                    }
    //                    if (newScrollY != oldScrollY) {
    //                        base.scrollTo(mScrollX, newScrollY);
    //                        return true;
    //                    }
    //                }
    //                break;
    //        }
    //        return base.onGenericMotionEvent(e);
    //    }
    //    //@Override
    //    protected void onOverScrolled(int scrollX, int scrollY,
    //            Boolean clampedX, Boolean clampedY) {
    //        // Treat animating scrolls differently; see #computeScroll() for why.
    //        if (!mScroller.isFinished()) {
    //            final int oldX = mScrollX;
    //            final int oldY = mScrollY;
    //            mScrollX = scrollX;
    //            mScrollY = scrollY;
    //            invalidateParentIfNeeded();
    //            onScrollChanged(mScrollX, mScrollY, oldX, oldY);
    //            if (clampedY) {
    //                mScroller.springBack(mScrollX, mScrollY, 0, 0, 0, getScrollRange());
    //            }
    //        } else {
    //            base.scrollTo(scrollX, scrollY);
    //        }
    //        awakenScrollBars();
    //    }
    //    /** @hide */
    //    //@Override
    //    public Boolean performAccessibilityActionInternal(int action, Bundle arguments) {
    //        if (base.performAccessibilityActionInternal(action, arguments)) {
    //            return true;
    //        }
    //        if (!isEnabled()) {
    //            return false;
    //        }
    //        switch (action) {
    //            case AccessibilityNodeInfo.ACTION_SCROLL_FORWARD:
    //            case R.id.accessibilityActionScrollDown: {
    //                final int viewportHeight = Height - PaddingBottom - PaddingTop;
    //                final int targetScrollY = Math.min(mScrollY + viewportHeight, getScrollRange());
    //                if (targetScrollY != mScrollY) {
    //                    smoothScrollTo(0, targetScrollY);
    //                    return true;
    //                }
    //            } return false;
    //            case AccessibilityNodeInfo.ACTION_SCROLL_BACKWARD:
    //            case R.id.accessibilityActionScrollUp: {
    //                final int viewportHeight = Height - PaddingBottom - PaddingTop;
    //                final int targetScrollY = Math.max(mScrollY - viewportHeight, 0);
    //                if (targetScrollY != mScrollY) {
    //                    smoothScrollTo(0, targetScrollY);
    //                    return true;
    //                }
    //            } return false;
    //        }
    //        return false;
    //    }
    //    //@Override
    //    public CharSequence getAccessibilityClassName() {
    //        return ScrollView.class.getName();
    //    }
    //    /** @hide */
    //    //@Override
    //    public void onInitializeAccessibilityNodeInfoInternal(AccessibilityNodeInfo info) {
    //        base.onInitializeAccessibilityNodeInfoInternal(info);
    //        if (isEnabled()) {
    //            final int scrollRange = getScrollRange();
    //            if (scrollRange > 0) {
    //                info.setScrollable(true);
    //                if (mScrollY > 0) {
    //                    info.addAction(
    //                            AccessibilityNodeInfo.AccessibilityAction.ACTION_SCROLL_BACKWARD);
    //                    info.addAction(AccessibilityNodeInfo.AccessibilityAction.ACTION_SCROLL_UP);
    //                }
    //                if (mScrollY < scrollRange) {
    //                    info.addAction(AccessibilityNodeInfo.AccessibilityAction.ACTION_SCROLL_FORWARD);
    //                    info.addAction(AccessibilityNodeInfo.AccessibilityAction.ACTION_SCROLL_DOWN);
    //                }
    //            }
    //        }
    //    }
    //    /** @hide */
    //    //@Override
    //    public void onInitializeAccessibilityEventInternal(AccessibilityEvent e) {
    //        base.onInitializeAccessibilityEventInternal(e);
    //        final Boolean scrollable = getScrollRange() > 0;
    //        e.setScrollable(scrollable);
    //        e.setScrollX(mScrollX);
    //        e.setScrollY(mScrollY);
    //        e.setMaxScrollX(mScrollX);
    //        e.setMaxScrollY(getScrollRange());
    //    }
    //    private int getScrollRange() {
    //        int scrollRange = 0;
    //        if (ChildCount > 0) {
    //            View child = GetChildAt(0);
    //            scrollRange = Math.max(0,
    //                    child.Height - (Height - PaddingBottom - PaddingTop));
    //        }
    //        return scrollRange;
    //    }
    //    /**
    //     * <p>
    //     * Finds the next focusable component that fits in the specified bounds.
    //     * </p>
    //     *
    //     * @param topFocus look for a candidate is the one at the top of the bounds
    //     *                 if topFocus is true, or at the bottom of the bounds if topFocus is
    //     *                 false
    //     * @param top      the top offset of the bounds in which a focusable must be
    //     *                 found
    //     * @param bottom   the bottom offset of the bounds in which a focusable must
    //     *                 be found
    //     * @return the next focusable component in the bounds or null if none can
    //     *         be found
    //     */
    //    private View findFocusableViewInBounds(Boolean topFocus, int top, int bottom) {
    //        List<View> focusables = getFocusables(View.FOCUS_FORWARD);
    //        View focusCandidate = null;
    //        /*
    //         * A fully contained focusable is one where its top is below the bound's
    //         * top, and its bottom is above the bound's bottom. A partially
    //         * contained focusable is one where some part of it is within the
    //         * bounds, but it also has some part that is not within bounds.  A fully contained
    //         * focusable is preferred to a partially contained focusable.
    //         */
    //        Boolean foundFullyContainedFocusable = false;
    //        int count = focusables.size();
    //        for (int i = 0; i < count; i++) {
    //            View view = focusables.get(i);
    //            int viewTop = view.getTop();
    //            int viewBottom = view.Bottom;
    //            if (top < viewBottom && viewTop < bottom) {
    //                /*
    //                 * the focusable is in the target area, it is a candidate for
    //                 * focusing
    //                 */
    //                final Boolean viewIsFullyContained = (top < viewTop) &&
    //                        (viewBottom < bottom);
    //                if (focusCandidate == null) {
    //                    /* No candidate, take this one */
    //                    focusCandidate = view;
    //                    foundFullyContainedFocusable = viewIsFullyContained;
    //                } else {
    //                    final Boolean viewIsCloserToBoundary =
    //                            (topFocus && viewTop < focusCandidate.getTop()) ||
    //                                    (!topFocus && viewBottom > focusCandidate
    //                                            .Bottom);
    //                    if (foundFullyContainedFocusable) {
    //                        if (viewIsFullyContained && viewIsCloserToBoundary) {
    //                            /*
    //                             * We're dealing with only fully contained views, so
    //                             * it has to be closer to the boundary to beat our
    //                             * candidate
    //                             */
    //                            focusCandidate = view;
    //                        }
    //                    } else {
    //                        if (viewIsFullyContained) {
    //                            /* Any fully contained view beats a partially contained view */
    //                            focusCandidate = view;
    //                            foundFullyContainedFocusable = true;
    //                        } else if (viewIsCloserToBoundary) {
    //                            /*
    //                             * Partially contained view beats another partially
    //                             * contained view if it's closer
    //                             */
    //                            focusCandidate = view;
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        return focusCandidate;
    //    }
    //    /**
    //     * <p>Handles scrolling in response to a "page up/down" shortcut press. This
    //     * method will scroll the view by one page up or down and give the focus
    //     * to the topmost/bottommost component in the new visible area. If no
    //     * component is a good candidate for focus, this scrollview reclaims the
    //     * focus.</p>
    //     *
    //     * @param direction the scroll direction: {@link android.view.View#FOCUS_UP}
    //     *                  to go one page up or
    //     *                  {@link android.view.View#FOCUS_DOWN} to go one page down
    //     * @return true if the key e is consumed by this method, false otherwise
    //     */
    //    public Boolean pageScroll(int direction) {
    //        Boolean down = direction == View.FOCUS_DOWN;
    //        int height = Height;
    //        if (down) {
    //            mTempRect.top = ScrollY + height;
    //            int count = ChildCount;
    //            if (count > 0) {
    //                View view = GetChildAt(count - 1);
    //                if (mTempRect.top + height > view.Bottom) {
    //                    mTempRect.top = view.Bottom - height;
    //                }
    //            }
    //        } else {
    //            mTempRect.top = ScrollY - height;
    //            if (mTempRect.top < 0) {
    //                mTempRect.top = 0;
    //            }
    //        }
    //        mTempRect.bottom = mTempRect.top + height;
    //        return scrollAndFocus(direction, mTempRect.top, mTempRect.bottom);
    //    }
    //    /**
    //     * <p>Handles scrolling in response to a "home/end" shortcut press. This
    //     * method will scroll the view to the top or bottom and give the focus
    //     * to the topmost/bottommost component in the new visible area. If no
    //     * component is a good candidate for focus, this scrollview reclaims the
    //     * focus.</p>
    //     *
    //     * @param direction the scroll direction: {@link android.view.View#FOCUS_UP}
    //     *                  to go the top of the view or
    //     *                  {@link android.view.View#FOCUS_DOWN} to go the bottom
    //     * @return true if the key e is consumed by this method, false otherwise
    //     */
    public Boolean fullScroll(int direction)
    {
        Boolean down = direction == FOCUS_DOWN;
        int height = Height;
        mTempRect.Top = 0;
        mTempRect.Bottom = height;
        if (down)
        {
            int count = ChildCount;
            if (count > 0)
            {
                View view = GetChildAt(count - 1);
                mTempRect.Bottom = view.Bottom + PaddingBottom;
                mTempRect.Top = mTempRect.Bottom - height;
            }
        }
        return scrollAndFocus(direction, mTempRect.Top, mTempRect.Bottom);
    }
    //    /**
    //     * <p>Scrolls the view to make the area defined by <code>top</code> and
    //     * <code>bottom</code> visible. This method attempts to give the focus
    //     * to a component visible in this area. If no component can be focused in
    //     * the new visible area, the focus is reclaimed by this ScrollView.</p>
    //     *
    //     * @param direction the scroll direction: {@link android.view.View#FOCUS_UP}
    //     *                  to go upward, {@link android.view.View#FOCUS_DOWN} to downward
    //     * @param top       the top offset of the new area to be made visible
    //     * @param bottom    the bottom offset of the new area to be made visible
    //     * @return true if the key e is consumed by this method, false otherwise
    //     */
    private Boolean scrollAndFocus(int direction, int top, int bottom)
    {
        Boolean handled = true;
        int height = Height;
        int containerTop = ScrollY;
        int containerBottom = containerTop + height;
        Boolean up = direction == FOCUS_UP;
        View newFocused = findFocusableViewInBounds(up, top, bottom);
        if (newFocused == null)
        {
            newFocused = this;
        }
        if (top >= containerTop && bottom <= containerBottom)
        {
            handled = false;
        }
        else
        {
            int delta = up ? (top - containerTop) : (bottom - containerBottom);
            doScrollY(delta);
        }
        if (newFocused != FindFocus()) newFocused.RequestFocus((FocusSearchDirection)direction);
        return handled;
    }
    /**
     * Handle scrolling in response to an up or down arrow click.
     *
     * @param direction The direction corresponding to the arrow key that was
     *                  pressed
     * @return True if we consumed the e, false otherwise
     */
    public Boolean arrowScroll(int direction)
    {
        View currentFocused = FindFocus();
        if (currentFocused == this) currentFocused = null;
        View nextFocused = FocusFinder.Instance.FindNextFocus(this, currentFocused, (FocusSearchDirection)direction);
        var maxJump = getMaxScrollAmount();
        if (nextFocused != null && isWithinDeltaOfScreen(nextFocused, maxJump, Height))
        {
            nextFocused.GetDrawingRect(mTempRect);
            OffsetDescendantRectToMyCoords(nextFocused, mTempRect);
            int scrollDelta = computeScrollDeltaToGetChildRectOnScreen(mTempRect);
            doScrollY(scrollDelta);
            nextFocused.RequestFocus(direction);
        }
        else
        {
            // no new focus
            int scrollDelta = maxJump;
            if (direction == FOCUS_UP && ScrollY < scrollDelta)
            {
                scrollDelta = ScrollY;
            }
            else if (direction == FOCUS_DOWN)
            {
                if (ChildCount > 0)
                {
                    int daBottom = GetChildAt(0).Bottom;
                    int screenBottom = ScrollY + Height - PaddingBottom;
                    if (daBottom - screenBottom < maxJump)
                    {
                        scrollDelta = daBottom - screenBottom;
                    }
                }
            }
            if (scrollDelta == 0)
            {
                return false;
            }
            doScrollY(direction == FOCUS_DOWN ? scrollDelta : -scrollDelta);
        }
        if (currentFocused != null && currentFocused.IsFocused
                && isOffScreen(currentFocused))
        {
            // previously focused item still has focus and is off screen, give
            // it up (take it back to ourselves)
            // (also, need to temporarily force FOCUS_BEFORE_DESCENDANTS so we are
            // sure to
            // get it)
            var descendantFocusability = DescendantFocusability;  // save
            DescendantFocusability = DescendantFocusability.BeforeDescendants;
            
            RequestFocus();
            DescendantFocusability = descendantFocusability; // restore
        }
        return true;
    }
    //    /**
    //     * @return whether the descendant of this scroll view is scrolled off
    //     *  screen.
    //     */
    //    private Boolean isOffScreen(View descendant) {
    //        return !isWithinDeltaOfScreen(descendant, 0, Height);
    //    }
    /**
     * @return whether the descendant of this scroll view is within delta
     *  pixels of being on the screen.
     */
    private Boolean isWithinDeltaOfScreen(View descendant, int delta, int height)
    {
        descendant.GetDrawingRect(mTempRect);
        OffsetDescendantRectToMyCoords(descendant, mTempRect);
        return (mTempRect.Bottom + delta) >= ScrollY
                && (mTempRect.Top - delta) <= (ScrollY + height);
    }
    //    /**
    //     * Smooth scroll by a Y delta
    //     *
    //     * @param delta the number of pixels to scroll by on the Y axis
    //     */
    //    private void doScrollY(int delta) {
    //        if (delta != 0) {
    //            if (mSmoothScrollingEnabled) {
    //                smoothScrollBy(0, delta);
    //            } else {
    //                scrollBy(0, delta);
    //            }
    //        }
    //    }
    //    /**
    //     * Like {@link View#scrollBy}, but scroll smoothly instead of immediately.
    //     *
    //     * @param dx the number of pixels to scroll by on the X axis
    //     * @param dy the number of pixels to scroll by on the Y axis
    //     */
    //    public final void smoothScrollBy(int dx, int dy) {
    //        if (ChildCount == 0) {
    //            // Nothing to do.
    //            return;
    //        }
    //        long duration = AnimationUtils.currentAnimationTimeMillis() - mLastScroll;
    //        if (duration > ANIMATED_SCROLL_GAP) {
    //            final int height = Height - PaddingBottom - PaddingTop;
    //            final int bottom = GetChildAt(0).Height;
    //            final int maxY = Math.max(0, bottom - height);
    //            final int scrollY = mScrollY;
    //            dy = Math.max(0, Math.min(scrollY + dy, maxY)) - scrollY;
    //            mScroller.startScroll(mScrollX, scrollY, 0, dy);
    //            postInvalidateOnAnimation();
    //        } else {
    //            if (!mScroller.isFinished()) {
    //                mScroller.abortAnimation();
    //                if (mFlingStrictSpan != null) {
    //                    mFlingStrictSpan.finish();
    //                    mFlingStrictSpan = null;
    //                }
    //            }
    //            scrollBy(dx, dy);
    //        }
    //        mLastScroll = AnimationUtils.currentAnimationTimeMillis();
    //    }
    //    /**
    //     * Like {@link #scrollTo}, but scroll smoothly instead of immediately.
    //     *
    //     * @param x the position where to scroll on the X axis
    //     * @param y the position where to scroll on the Y axis
    //     */
    //    public final void smoothScrollTo(int x, int y) {
    //        smoothScrollBy(x - mScrollX, y - mScrollY);
    //    }
    //    /**
    //     * <p>The scroll range of a scroll view is the overall height of all of its
    //     * children.</p>
    //     */
    //    //@Override
    //    protected int computeVerticalScrollRange() {
    //        final int count = ChildCount;
    //        final int contentHeight = Height - PaddingBottom - PaddingTop;
    //        if (count == 0) {
    //            return contentHeight;
    //        }
    //        int scrollRange = GetChildAt(0).Bottom;
    //        final int scrollY = mScrollY;
    //        final int overscrollBottom = Math.max(0, scrollRange - contentHeight);
    //        if (scrollY < 0) {
    //            scrollRange -= scrollY;
    //        } else if (scrollY > overscrollBottom) {
    //            scrollRange += scrollY - overscrollBottom;
    //        }
    //        return scrollRange;
    //    }
    //    //@Override
    //    protected int computeVerticalScrollOffset() {
    //        return Math.max(0, base.computeVerticalScrollOffset());
    //    }
    //    //@Override
    //    protected void measureChild(View child, int parentWidthMeasureSpec,
    //            int parentHeightMeasureSpec) {
    //        ViewGroup.LayoutParams lp = child.getLayoutParams();
    //        int childWidthMeasureSpec;
    //        int childHeightMeasureSpec;
    //        childWidthMeasureSpec = getChildMeasureSpec(parentWidthMeasureSpec, PaddingLeft
    //                + PaddingRight, lp.width);
    //        final int verticalPadding = PaddingTop + PaddingBottom;
    //        childHeightMeasureSpec = View.MeasureSpec.makeSafeMeasureSpec(
    //                Math.max(0, View.MeasureSpec.getSize(parentHeightMeasureSpec) - verticalPadding),
    //                View.MeasureSpec.UNSPECIFIED);
    //        child.measure(childWidthMeasureSpec, childHeightMeasureSpec);
    //    }
    //    //@Override
    //    protected void measureChildWithMargins(View child, int parentWidthMeasureSpec, int widthUsed,
    //            int parentHeightMeasureSpec, int heightUsed) {
    //        final ViewGroup.MarginLayoutParams lp = (ViewGroup.MarginLayoutParams) child.getLayoutParams();
    //        final int childWidthMeasureSpec = getChildMeasureSpec(parentWidthMeasureSpec,
    //                PaddingLeft + PaddingRight + lp.leftMargin + lp.rightMargin
    //                        + widthUsed, lp.width);
    //        final int usedTotal = PaddingTop + PaddingBottom + lp.topMargin + lp.bottomMargin +
    //                heightUsed;
    //        final int childHeightMeasureSpec = View.MeasureSpec.makeSafeMeasureSpec(
    //                Math.max(0, View.MeasureSpec.getSize(parentHeightMeasureSpec) - usedTotal),
    //                View.MeasureSpec.UNSPECIFIED);
    //        child.measure(childWidthMeasureSpec, childHeightMeasureSpec);
    //    }
    //    //@Override
    //    public void computeScroll() {
    //        if (mScroller.computeScrollOffset()) {
    //            // This is called at drawing time by ViewGroup.  We don't want to
    //            // re-show the scrollbars at this point, which scrollTo will do,
    //            // so we replicate most of scrollTo here.
    //            //
    //            //         It's a little odd to call onScrollChanged from inside the drawing.
    //            //
    //            //         It is, except when you remember that computeScroll() is used to
    //            //         animate scrolling. So unless we want to defer the onScrollChanged()
    //            //         until the end of the animated scrolling, we don't really have a
    //            //         choice here.
    //            //
    //            //         I agree.  The alternative, which I think would be worse, is to post
    //            //         something and tell the subclasses later.  This is bad because there
    //            //         will be a window where mScrollX/Y is different from what the app
    //            //         thinks it is.
    //            //
    //            int oldX = mScrollX;
    //            int oldY = mScrollY;
    //            int x = mScroller.getCurrX();
    //            int y = mScroller.getCurrY();
    //            if (oldX != x || oldY != y) {
    //                final int range = getScrollRange();
    //                final int overscrollMode = getOverScrollMode();
    //                final Boolean canOverscroll = overscrollMode == OVER_SCROLL_ALWAYS ||
    //                        (overscrollMode == OVER_SCROLL_IF_CONTENT_SCROLLS && range > 0);
    //                overScrollBy(x - oldX, y - oldY, oldX, oldY, 0, range,
    //                        0, mOverflingDistance, false);
    //                onScrollChanged(mScrollX, mScrollY, oldX, oldY);
    //                if (canOverscroll) {
    //                    if (y < 0 && oldY >= 0) {
    //                        mEdgeGlowTop.onAbsorb((int) mScroller.getCurrVelocity());
    //                    } else if (y > range && oldY <= range) {
    //                        mEdgeGlowBottom.onAbsorb((int) mScroller.getCurrVelocity());
    //                    }
    //                }
    //            }
    //            if (!awakenScrollBars()) {
    //                // Keep on drawing until the animation has finished.
    //                postInvalidateOnAnimation();
    //            }
    //        } else {
    //            if (mFlingStrictSpan != null) {
    //                mFlingStrictSpan.finish();
    //                mFlingStrictSpan = null;
    //            }
    //        }
    //    }
    //    /**
    //     * Scrolls the view to the given child.
    //     *
    //     * @param child the View to scroll to
    //     */
    //    public void scrollToDescendant(//@NonNull View child) {
    //        if (!mIsLayoutDirty) {
    //            child.getDrawingRect(mTempRect);
    //            /* Offset from child's local coordinates to ScrollView coordinates */
    //            offsetDescendantRectToMyCoords(child, mTempRect);
    //            int scrollDelta = computeScrollDeltaToGetChildRectOnScreen(mTempRect);
    //            if (scrollDelta != 0) {
    //                scrollBy(0, scrollDelta);
    //            }
    //        } else {
    //            mChildToScrollTo = child;
    //        }
    //    }
    //    /**
    //     * If rect is off screen, scroll just enough to get it (or at least the
    //     * first screen size chunk of it) on screen.
    //     *
    //     * @param rect      The rectangle.
    //     * @param immediate True to scroll immediately without animation
    //     * @return true if scrolling was performed
    //     */
    //    private Boolean scrollToChildRect(Rect rect, Boolean immediate) {
    //        final int delta = computeScrollDeltaToGetChildRectOnScreen(rect);
    //        final Boolean scroll = delta != 0;
    //        if (scroll) {
    //            if (immediate) {
    //                scrollBy(0, delta);
    //            } else {
    //                smoothScrollBy(0, delta);
    //            }
    //        }
    //        return scroll;
    //    }
    //    /**
    //     * Compute the amount to scroll in the Y direction in order to get
    //     * a rectangle completely on the screen (or, if taller than the screen,
    //     * at least the first screen size chunk of it).
    //     *
    //     * @param rect The rect.
    //     * @return The scroll delta.
    //     */
    //    protected int computeScrollDeltaToGetChildRectOnScreen(Rect rect) {
    //        if (ChildCount == 0) return 0;
    //        int height = Height;
    //        int screenTop = ScrollY;
    //        int screenBottom = screenTop + height;
    //        int fadingEdge = getVerticalFadingEdgeLength();
    //        // leave room for top fading edge as long as rect isn't at very top
    //        if (rect.top > 0) {
    //            screenTop += fadingEdge;
    //        }
    //        // leave room for bottom fading edge as long as rect isn't at very bottom
    //        if (rect.bottom < GetChildAt(0).Height) {
    //            screenBottom -= fadingEdge;
    //        }
    //        int scrollYDelta = 0;
    //        if (rect.bottom > screenBottom && rect.top > screenTop) {
    //            // need to move down to get it in view: move down just enough so
    //            // that the entire rectangle is in view (or at least the first
    //            // screen size chunk).
    //            if (rect.height() > height) {
    //                // just enough to get screen size chunk on
    //                scrollYDelta += (rect.top - screenTop);
    //            } else {
    //                // get entire rect at bottom of screen
    //                scrollYDelta += (rect.bottom - screenBottom);
    //            }
    //            // make sure we aren't scrolling beyond the end of our content
    //            int bottom = GetChildAt(0).Bottom;
    //            int distanceToBottom = bottom - screenBottom;
    //            scrollYDelta = Math.min(scrollYDelta, distanceToBottom);
    //        } else if (rect.top < screenTop && rect.bottom < screenBottom) {
    //            // need to move up to get it in view: move up just enough so that
    //            // entire rectangle is in view (or at least the first screen
    //            // size chunk of it).
    //            if (rect.height() > height) {
    //                // screen size chunk
    //                scrollYDelta -= (screenBottom - rect.bottom);
    //            } else {
    //                // entire rect at top
    //                scrollYDelta -= (screenTop - rect.top);
    //            }
    //            // make sure we aren't scrolling any further than the top our content
    //            scrollYDelta = Math.max(scrollYDelta, -ScrollY);
    //        }
    //        return scrollYDelta;
    //    }
    //    //@Override
    //    public void requestChildFocus(View child, View focused) {
    //        if (focused != null && focused.getRevealOnFocusHint()) {
    //            if (!mIsLayoutDirty) {
    //                scrollToDescendant(focused);
    //            } else {
    //                // The child may not be laid out yet, we can't compute the scroll yet
    //                mChildToScrollTo = focused;
    //            }
    //        }
    //        base.requestChildFocus(child, focused);
    //    }
    //    /**
    //     * When looking for focus in children of a scroll view, need to be a little
    //     * more careful not to give focus to something that is scrolled off screen.
    //     *
    //     * This is more expensive than the default {@link android.view.ViewGroup}
    //     * implementation, otherwise this behavior might have been made the default.
    //     */
    //    //@Override
    //    protected Boolean onRequestFocusInDescendants(int direction,
    //            Rect previouslyFocusedRect) {
    //        // convert from forward / backward notation to up / down / left / right
    //        // (ugh).
    //        if (direction == View.FOCUS_FORWARD) {
    //            direction = View.FOCUS_DOWN;
    //        } else if (direction == View.FOCUS_BACKWARD) {
    //            direction = View.FOCUS_UP;
    //        }
    //        final View nextFocus = previouslyFocusedRect == null ?
    //                FocusFinder.getInstance().findNextFocus(this, null, direction) :
    //                FocusFinder.getInstance().findNextFocusFromRect(this,
    //                        previouslyFocusedRect, direction);
    //        if (nextFocus == null) {
    //            return false;
    //        }
    //        if (isOffScreen(nextFocus)) {
    //            return false;
    //        }
    //        return nextFocus.requestFocus(direction, previouslyFocusedRect);
    //    }
    //    //@Override
    //    public Boolean requestChildRectangleOnScreen(View child, Rect rectangle,
    //            Boolean immediate) {
    //        // offset into coordinate space of this scroll view
    //        rectangle.offset(child.getLeft() - child.getScrollX(),
    //                child.getTop() - child.ScrollY);
    //        return scrollToChildRect(rectangle, immediate);
    //    }
    //    //@Override
    //    public void requestLayout() {
    //        mIsLayoutDirty = true;
    //        base.requestLayout();
    //    }
    //    //@Override
    //    protected void onDetachedFromWindow() {
    //        base.onDetachedFromWindow();
    //        if (mScrollStrictSpan != null) {
    //            mScrollStrictSpan.finish();
    //            mScrollStrictSpan = null;
    //        }
    //        if (mFlingStrictSpan != null) {
    //            mFlingStrictSpan.finish();
    //            mFlingStrictSpan = null;
    //        }
    //    }
    //    //@Override
    //    protected void onLayout(Boolean changed, int l, int t, int r, int b) {
    //        base.onLayout(changed, l, t, r, b);
    //        mIsLayoutDirty = false;
    //        // Give a child focus if it needs it
    //        if (mChildToScrollTo != null && isViewDescendantOf(mChildToScrollTo, this)) {
    //            scrollToDescendant(mChildToScrollTo);
    //        }
    //        mChildToScrollTo = null;
    //        if (!isLaidOut()) {
    //            if (mSavedState != null) {
    //                mScrollY = mSavedState.scrollPosition;
    //                mSavedState = null;
    //            } // mScrollY default value is "0"
    //            final int childHeight = (ChildCount > 0) ? GetChildAt(0).getMeasuredHeight() : 0;
    //            final int scrollRange = Math.max(0,
    //                    childHeight - (b - t - PaddingBottom - PaddingTop));
    //            // Don't forget to clamp
    //            if (mScrollY > scrollRange) {
    //                mScrollY = scrollRange;
    //            } else if (mScrollY < 0) {
    //                mScrollY = 0;
    //            }
    //        }
    //        // Calling this with the present values causes it to re-claim them
    //        scrollTo(mScrollX, mScrollY);
    //    }
    //    //@Override
    //    protected void onSizeChanged(int w, int h, int oldw, int oldh) {
    //        base.onSizeChanged(w, h, oldw, oldh);
    //        View currentFocused = findFocus();
    //        if (null == currentFocused || this == currentFocused)
    //            return;
    //        // If the currently-focused view was visible on the screen when the
    //        // screen was at the old height, then scroll the screen to make that
    //        // view visible with the new screen height.
    //        if (isWithinDeltaOfScreen(currentFocused, 0, oldh)) {
    //            currentFocused.getDrawingRect(mTempRect);
    //            offsetDescendantRectToMyCoords(currentFocused, mTempRect);
    //            int scrollDelta = computeScrollDeltaToGetChildRectOnScreen(mTempRect);
    //            doScrollY(scrollDelta);
    //        }
    //    }
    //    /**
    //     * Return true if child is a descendant of parent, (or equal to the parent).
    //     */
    //    private static Boolean isViewDescendantOf(View child, View parent) {
    //        if (child == parent) {
    //            return true;
    //        }
    //        final ViewParent theParent = child.getParent();
    //        return (theParent instanceof ViewGroup) && isViewDescendantOf((View) theParent, parent);
    //    }
    //    /**
    //     * Fling the scroll view
    //     *
    //     * @param velocityY The initial velocity in the Y direction. Positive
    //     *                  numbers mean that the finger/cursor is moving down the screen,
    //     *                  which means we want to scroll towards the top.
    //     */
    //    public void fling(int velocityY) {
    //        if (ChildCount > 0) {
    //            int height = Height - PaddingBottom - PaddingTop;
    //            int bottom = GetChildAt(0).Height;
    //            mScroller.fling(mScrollX, mScrollY, 0, velocityY, 0, 0, 0,
    //                    Math.max(0, bottom - height), 0, height/2);
    //            if (mFlingStrictSpan == null) {
    //                mFlingStrictSpan = StrictMode.enterCriticalSpan("ScrollView-fling");
    //            }
    //            postInvalidateOnAnimation();
    //        }
    //    }
    //    private void flingWithNestedDispatch(int velocityY) {
    //        final Boolean canFling = (mScrollY > 0 || velocityY > 0) &&
    //                (mScrollY < getScrollRange() || velocityY < 0);
    //        if (!dispatchNestedPreFling(0, velocityY)) {
    //            dispatchNestedFling(0, velocityY, canFling);
    //            if (canFling) {
    //                fling(velocityY);
    //            }
    //        }
    //    }
    //    //@UnsupportedAppUsage
    //    private void endDrag() {
    //        mIsBeingDragged = false;
    //        recycleVelocityTracker();
    //        if (shouldDisplayEdgeEffects()) {
    //            mEdgeGlowTop.onRelease();
    //            mEdgeGlowBottom.onRelease();
    //        }
    //        if (mScrollStrictSpan != null) {
    //            mScrollStrictSpan.finish();
    //            mScrollStrictSpan = null;
    //        }
    //    }
    //    /**
    //     * {@inheritDoc}
    //     *
    //     * <p>This version also clamps the scrolling to the bounds of our child.
    //     */
    //    //@Override
    //    public void scrollTo(int x, int y) {
    //        // we rely on the fact the View.scrollBy calls scrollTo.
    //        if (ChildCount > 0) {
    //            View child = GetChildAt(0);
    //            x = clamp(x, getWidth() - PaddingRight - PaddingLeft, child.getWidth());
    //            y = clamp(y, Height - PaddingBottom - PaddingTop, child.Height);
    //            if (x != mScrollX || y != mScrollY) {
    //                base.scrollTo(x, y);
    //            }
    //        }
    //    }
    //    //@Override
    //    public Boolean onStartNestedScroll(View child, View target, int nestedScrollAxes) {
    //        return (nestedScrollAxes & SCROLL_AXIS_VERTICAL) != 0;
    //    }
    //    //@Override
    //    public void onNestedScrollAccepted(View child, View target, int axes) {
    //        base.onNestedScrollAccepted(child, target, axes);
    //        startNestedScroll(SCROLL_AXIS_VERTICAL);
    //    }
    //    /**
    //     * @inheritDoc
    //     */
    //    //@Override
    //    public void onStopNestedScroll(View target) {
    //        base.onStopNestedScroll(target);
    //    }
    //    //@Override
    //    public void onNestedScroll(View target, int dxConsumed, int dyConsumed,
    //            int dxUnconsumed, int dyUnconsumed) {
    //        final int oldScrollY = mScrollY;
    //        scrollBy(0, dyUnconsumed);
    //        final int myConsumed = mScrollY - oldScrollY;
    //        final int myUnconsumed = dyUnconsumed - myConsumed;
    //        dispatchNestedScroll(0, myConsumed, 0, myUnconsumed, null);
    //    }
    //    /**
    //     * @inheritDoc
    //     */
    //    //@Override
    //    public Boolean onNestedFling(View target, float velocityX, float velocityY, Boolean consumed) {
    //        if (!consumed) {
    //            flingWithNestedDispatch((int) velocityY);
    //            return true;
    //        }
    //        return false;
    //    }
    //    //@Override
    //    public void draw(Canvas canvas) {
    //        base.draw(canvas);
    //        if (shouldDisplayEdgeEffects()) {
    //            final int scrollY = mScrollY;
    //            final Boolean clipToPadding = getClipToPadding();
    //            if (!mEdgeGlowTop.isFinished()) {
    //                final int restoreCount = canvas.save();
    //                final int width;
    //                final int height;
    //                final float translateX;
    //                final float translateY;
    //                if (clipToPadding) {
    //                    width = getWidth() - PaddingLeft - PaddingRight;
    //                    height = Height - PaddingTop - PaddingBottom;
    //                    translateX = PaddingLeft;
    //                    translateY = PaddingTop;
    //                } else {
    //                    width = getWidth();
    //                    height = Height;
    //                    translateX = 0;
    //                    translateY = 0;
    //                }
    //                canvas.translate(translateX, Math.min(0, scrollY) + translateY);
    //                mEdgeGlowTop.setSize(width, height);
    //                if (mEdgeGlowTop.draw(canvas)) {
    //                    postInvalidateOnAnimation();
    //                }
    //                canvas.restoreToCount(restoreCount);
    //            }
    //            if (!mEdgeGlowBottom.isFinished()) {
    //                final int restoreCount = canvas.save();
    //                final int width;
    //                final int height;
    //                final float translateX;
    //                final float translateY;
    //                if (clipToPadding) {
    //                    width = getWidth() - PaddingLeft - PaddingRight;
    //                    height = Height - PaddingTop - PaddingBottom;
    //                    translateX = PaddingLeft;
    //                    translateY = PaddingTop;
    //                } else {
    //                    width = getWidth();
    //                    height = Height;
    //                    translateX = 0;
    //                    translateY = 0;
    //                }
    //                canvas.translate(-width + translateX,
    //                            Math.max(getScrollRange(), scrollY) + height + translateY);
    //                canvas.rotate(180, width, 0);
    //                mEdgeGlowBottom.setSize(width, height);
    //                if (mEdgeGlowBottom.draw(canvas)) {
    //                    postInvalidateOnAnimation();
    //                }
    //                canvas.restoreToCount(restoreCount);
    //            }
    //        }
    //    }
    //    private static int clamp(int n, int my, int child) {
    //        if (my >= child || n < 0) {
    //            /* my >= child is this case:
    //             *                    |--------------- me ---------------|
    //             *     |------ child ------|
    //             * or
    //             *     |--------------- me ---------------|
    //             *            |------ child ------|
    //             * or
    //             *     |--------------- me ---------------|
    //             *                                  |------ child ------|
    //             *
    //             * n < 0 is this case:
    //             *     |------ me ------|
    //             *                    |-------- child --------|
    //             *     |-- mScrollX --|
    //             */
    //            return 0;
    //        }
    //        if ((my+n) > child) {
    //            /* this case:
    //             *                    |------ me ------|
    //             *     |------ child ------|
    //             *     |-- mScrollX --|
    //             */
    //            return child-my;
    //        }
    //        return n;
    //    }
    //    //@Override
    //    protected void onRestoreInstanceState(Parcelable state) {
    //        if (mContext.getApplicationInfo().targetSdkVersion <= Build.VERSION_CODES.JELLY_BEAN_MR2) {
    //            // Some old apps reused IDs in ways they shouldn't have.
    //            // Don't break them, but they don't get scroll state restoration.
    //            base.onRestoreInstanceState(state);
    //            return;
    //        }
    //        SavedState ss = (SavedState) state;
    //        base.onRestoreInstanceState(ss.getSuperState());
    //        mSavedState = ss;
    //        requestLayout();
    //    }
    //    //@Override
    //    protected Parcelable onSaveInstanceState() {
    //        if (mContext.getApplicationInfo().targetSdkVersion <= Build.VERSION_CODES.JELLY_BEAN_MR2) {
    //            // Some old apps reused IDs in ways they shouldn't have.
    //            // Don't break them, but they don't get scroll state restoration.
    //            return base.onSaveInstanceState();
    //        }
    //        Parcelable superState = base.onSaveInstanceState();
    //        SavedState ss = new SavedState(superState);
    //        ss.scrollPosition = mScrollY;
    //        return ss;
    //    }
    //    /** @hide */
    //    //@Override
    //    protected void encodeProperties(//@NonNull ViewHierarchyEncoder encoder) {
    //        base.encodeProperties(encoder);
    //        encoder.addProperty("fillViewport", mFillViewport);
    //    }
    private class SavedState : BaseSavedState
    {
        public int scrollPosition;
        public SavedState(Parcel superState)
        : base(superState)
        {
            
        }
        //public SavedState(Parcel source) : base(source)
        //{
            
        //    scrollPosition = source.ReadInt();
        //}
        //@Override
        public override void WriteToParcel(Parcel dest, ParcelableWriteFlags flags)
        {
            base.WriteToParcel(dest, flags);
            dest.WriteInt(scrollPosition);
        }
        //@Override
        public String toString()
        {
            return "ScrollView.SavedState{"
                    + Integer.ToHexString(JniIdentityHashCode)
                    + " scrollPosition=" + scrollPosition + "}";
        }
        //public const @android.annotation.NonNull Parcelable.Creator<SavedState> CREATOR
        //        = new Parcelable.Creator<SavedState>()
        //        {
        //    public SavedState createFromParcel(Parcel i)
        //{
        //    return new SavedState(i);
        //}
        public SavedState[] newArray(int size)
        {
            return new SavedState[size];
        }
    };
}

