
using System;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Views.Accessibility;
using Android.Views.Animations;
using Android.Widget;

namespace XamarinAndroidNativeTest
{

	public class ScrollView4 : FrameLayout
	{
		internal const int ANIMATED_SCROLL_GAP = 250;

		internal const float MAX_SCROLL_FACTOR = 0.5f;

		private long mLastScroll;

		private readonly Rect mTempRect = new Rect();

		private OverScroller3 mScroller;

		private EdgeEffect mEdgeGlowTop;

		private EdgeEffect mEdgeGlowBottom;

		/// <summary>Position of the last motion event.</summary>
		/// <remarks>Position of the last motion event.</remarks>
		private float mLastMotionY;

		/// <summary>True when the layout has changed but the traversal has not come through yet.
		/// 	</summary>
		/// <remarks>
		/// True when the layout has changed but the traversal has not come through yet.
		/// Ideally the view hierarchy would keep track of this for us.
		/// </remarks>
		private bool mIsLayoutDirty = true;

		/// <summary>
		/// The child to give focus to in the event that a child has requested focus while the
		/// layout is dirty.
		/// </summary>
		/// <remarks>
		/// The child to give focus to in the event that a child has requested focus while the
		/// layout is dirty. This prevents the scroll from being wrong if the child has not been
		/// laid out before requesting focus.
		/// </remarks>
		private View mChildToScrollTo = null;

		/// <summary>True if the user is currently dragging this ScrollView around.</summary>
		/// <remarks>
		/// True if the user is currently dragging this ScrollView around. This is
		/// not the same as 'is being flinged', which can be checked by
		/// mScroller.isFinished() (flinging begins when the user lifts his finger).
		/// </remarks>
		private bool mIsBeingDragged = false;

		/// <summary>Determines speed during touch scrolling</summary>
		private VelocityTracker mVelocityTracker;

		/// <summary>
		/// When set to true, the scroll view measure its child to make it fill the currently
		/// visible area.
		/// </summary>
		/// <remarks>
		/// When set to true, the scroll view measure its child to make it fill the currently
		/// visible area.
		/// </remarks>
		private bool mFillViewport;

		/// <summary>Whether arrow scrolling is animated.</summary>
		/// <remarks>Whether arrow scrolling is animated.</remarks>
		private bool mSmoothScrollingEnabled = true;

		private int mTouchSlop;

		private int mMinimumVelocity;

		private int mMaximumVelocity;

		private int mOverscrollDistance;

		private int mOverflingDistance;

		/// <summary>ID of the active pointer.</summary>
		/// <remarks>
		/// ID of the active pointer. This is used to retain consistency during
		/// drags/flings if multiple pointers are used.
		/// </remarks>
		private int mActivePointerId = INVALID_POINTER;

		/// <summary>
		/// The StrictMode "critical time span" objects to catch animation
		/// stutters.
		/// </summary>
		/// <remarks>
		/// The StrictMode "critical time span" objects to catch animation
		/// stutters.  Non-null when a time-sensitive animation is
		/// in-flight.  Must call finish() on them when done animating.
		/// These are no-ops on user builds.
		/// </remarks>
		private StrictMode2.Span mScrollStrictSpan = null;

		private StrictMode2.Span mFlingStrictSpan = null;

		/// <summary>Sentinel value for no current active pointer.</summary>
		/// <remarks>
		/// Sentinel value for no current active pointer.
		/// Used by
		/// <see cref="mActivePointerId">mActivePointerId</see>
		/// .
		/// </remarks>
		internal const int INVALID_POINTER = -1;

		public ScrollView4(Context context) : this(context, null)
		{
            mEdgeGlowTop = new EdgeEffect(Context);
            mEdgeGlowBottom = new EdgeEffect(Context);
		}

		public ScrollView4(Context context, IAttributeSet attrs
			) : this(context, attrs, 16842880)
		{
		}

		public ScrollView4(Context context, IAttributeSet attrs
			, int defStyle) : base(context, attrs, defStyle)
		{
			// aka "drag"
			initScrollView();
            Android.Content.Res.TypedArray a = context.ObtainStyledAttributes(attrs,
				new Int32[] {
                unchecked((int)(0x0102023c)),
                defStyle, 0});
            FillViewport = a.GetBoolean(0, false);
			
			a.Recycle();
		}

		//[Sharpen.OverridesMethod(@"ViewGroup")]
		public override bool ShouldDelayChildPressedState()
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
                int length = VerticalFadingEdgeLength;
                if (ScrollY < length)
                {
                    return ScrollY / (float)length;
                }
                return 1.0f;
            }
        }


        protected override Single BottomFadingEdgeStrength
        {
            get
            {
                if (ChildCount == 0)
                {
                    return 0.0f;
                }
                int length = VerticalFadingEdgeLength;
                int bottomEdge = Height - PaddingBottom;
                int span = GetChildAt(0).Bottom - ScrollY - bottomEdge;
                if (span < length)
                {
                    return span / (float)length;
                }
                return 1.0f;
            }
        }

       

		/// <returns>
		/// The maximum amount this scroll view will scroll in response to
		/// an arrow event.
		/// </returns>
		public virtual int getMaxScrollAmount()
		{
			return (int)(MAX_SCROLL_FACTOR * (Bottom - Top));
		}

		private void initScrollView()
		{
			mScroller = new OverScroller3(Context);
			Focusable = true;
			
            DescendantFocusability = DescendantFocusability.AfterDescendants;
			SetWillNotDraw(false);
			ViewConfiguration configuration = ViewConfiguration.Get(Context);
            mTouchSlop = configuration.ScaledTouchSlop;
            mMinimumVelocity = configuration.ScaledMinimumFlingVelocity;
			mMaximumVelocity = configuration.ScaledMaximumFlingVelocity;
			mOverscrollDistance = configuration.ScaledOverscrollDistance;
			mOverflingDistance = configuration.ScaledOverflingDistance;
		}

		//[Sharpen.OverridesMethod(@"ViewGroup")]
		public override void AddView(View child)
		{
			if (ChildCount > 0)
			{
				throw new System.InvalidOperationException("ScrollView can host only one direct child"
					);
			}
			base.AddView(child);
		}

		//[Sharpen.OverridesMethod(@"ViewGroup")]
		public override void AddView(View child, int index)
		{
			if (ChildCount > 0)
			{
				throw new System.InvalidOperationException("ScrollView can host only one direct child"
					);
			}
			base.AddView(child, index);
		}

		//[Sharpen.OverridesMethod(@"ViewGroup")]
		public override void AddView(View child, ViewGroup.LayoutParams
			 @params)
		{
			if (ChildCount > 0)
			{
				throw new System.InvalidOperationException("ScrollView can host only one direct child"
					);
			}
			base.AddView(child, @params);
		}

		//[Sharpen.OverridesMethod(@"ViewGroup")]
		public override void AddView(View child, int index, ViewGroup
			.LayoutParams @params)
		{
			if (ChildCount > 0)
			{
				throw new System.InvalidOperationException("ScrollView can host only one direct child"
					);
			}
			base.AddView(child, index, @params);
		}

		/// <returns>Returns true this ScrollView can be scrolled</returns>
		private bool canScroll()
		{
			View child = GetChildAt(0);
			if (child != null)
			{
				int childHeight = child.Height;
				return Height < childHeight + PaddingTop + PaddingBottom;
			}
			return false;
		}

		///// <summary>Indicates whether this ScrollView's content is stretched to fill the viewport.
		///// 	</summary>
		///// <remarks>Indicates whether this ScrollView's content is stretched to fill the viewport.
		///// 	</remarks>
		///// <returns>True if the content fills the viewport, false otherwise.</returns>
		///// <attr>ref android.R.Styleable#ScrollView_fillViewport</attr>
		//public virtual bool isFillViewport()
		//{
		//	return mFillViewport;
		//}

		public virtual Boolean FillViewport
        {
            get => mFillViewport;
            set
            {
                if (value != mFillViewport)
                {
                    mFillViewport = value;
                    RequestLayout();
                }
            }
        }

		///// <summary>
		///// Indicates this ScrollView whether it should stretch its content height to fill
		///// the viewport or not.
		///// </summary>
		///// <remarks>
		///// Indicates this ScrollView whether it should stretch its content height to fill
		///// the viewport or not.
		///// </remarks>
		///// <param name="fillViewport">
		///// True to stretch the content's height to the viewport's
		///// boundaries, false otherwise.
		///// </param>
		///// <attr>ref android.R.Styleable#ScrollView_fillViewport</attr>
		//public virtual void setFillViewport(bool fillViewport)
		//{
		//	if (fillViewport != mFillViewport)
		//	{
		//		mFillViewport = fillViewport;
		//		RequestLayout();
		//	}
		//}

		/// <returns>Whether arrow scrolling will animate its transition.</returns>
		public virtual bool isSmoothScrollingEnabled()
		{
			return mSmoothScrollingEnabled;
		}

		/// <summary>Set whether arrow scrolling will animate its transition.</summary>
		/// <remarks>Set whether arrow scrolling will animate its transition.</remarks>
		/// <param name="smoothScrollingEnabled">whether arrow scrolling will animate its transition
		/// 	</param>
		public virtual void setSmoothScrollingEnabled(bool smoothScrollingEnabled)
		{
			mSmoothScrollingEnabled = smoothScrollingEnabled;
		}

		//[Sharpen.OverridesMethod(@"View")]
		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec
			)
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
				View child = GetChildAt(0);
				int height = MeasuredHeight;
				if (child.MeasuredHeight < height)
				{
					FrameLayout.LayoutParams lp = (FrameLayout.LayoutParams
						)child.LayoutParameters;
					int childWidthMeasureSpec = GetChildMeasureSpec(widthMeasureSpec, PaddingLeft + 
						PaddingRight, lp.Width);
					height -= PaddingTop;
					height -= PaddingBottom;
					int childHeightMeasureSpec = View.MeasureSpec.MakeMeasureSpec(height
						, MeasureSpecMode.Exactly);
					child.Measure(childWidthMeasureSpec, childHeightMeasureSpec);
				}
			}
		}

		//[Sharpen.OverridesMethod(@"View")]
		public override bool DispatchKeyEvent(KeyEvent @event)
		{
			// Let the focused view and/or our descendants get the key first
			return base.DispatchKeyEvent(@event) || executeKeyEvent(@event);
		}

		/// <summary>
		/// You can call this function yourself to have the scroll view perform
		/// scrolling from a key event, just as if the event had been dispatched to
		/// it by the view hierarchy.
		/// </summary>
		/// <remarks>
		/// You can call this function yourself to have the scroll view perform
		/// scrolling from a key event, just as if the event had been dispatched to
		/// it by the view hierarchy.
		/// </remarks>
		/// <param name="event">The key event to execute.</param>
		/// <returns>Return true if the event was handled, else false.</returns>
		public virtual bool executeKeyEvent(KeyEvent @event)
		{
			mTempRect.SetEmpty();
			if (!canScroll())
			{
				if (IsFocused && @event.KeyCode != Keycode.Back)
				{
					View currentFocused = FindFocus();
					if (currentFocused == this)
					{
						currentFocused = null;
					}
					View nextFocused = FocusFinder.Instance.FindNextFocus
						(this, currentFocused, FocusSearchDirection.Down);
					return nextFocused != null && nextFocused != this && nextFocused.RequestFocus(FocusSearchDirection.Down);
				}
				return false;
			}
			bool handled = false;
			if (@event.Action == KeyEventActions.Down)
			{
				switch (@event.KeyCode)
				{
					case Keycode.DpadUp:
					{
						if (!@event.IsAltPressed)
						{
							handled = arrowScroll(0x00000021);
						}
						else
						{
							handled = fullScroll(0x00000021);
						}
						break;
					}

					case Keycode.DpadDown:
					{
						if (!@event.IsAltPressed)
						{
							handled = arrowScroll(0x00000082);
						}
						else
						{
							handled = fullScroll(0x00000082);
						}
						break;
					}

					case Keycode.Space:
					{
						pageScroll(@event.IsShiftPressed ? 0x00000021 : 0x00000082);
						break;
					}
				}
			}
			return handled;
		}

		private bool inChild(int x, int y)
		{
			if (ChildCount > 0)
			{
				int scrollY = ScrollY;
				View child = GetChildAt(0);
				return !(y < child.Top - scrollY || y >= child.Bottom - scrollY || x < 
					child.Left || x >= child.Right);
			}
			return false;
		}

		private void initOrResetVelocityTracker()
		{
			if (mVelocityTracker == null)
			{
				mVelocityTracker = VelocityTracker.Obtain();
			}
			else
			{
				mVelocityTracker.Clear();
			}
		}

		private void initVelocityTrackerIfNotExists()
		{
			if (mVelocityTracker == null)
			{
				mVelocityTracker = VelocityTracker.Obtain();
			}
		}

		private void RecycleVelocityTracker()
		{
			if (mVelocityTracker != null)
			{
				mVelocityTracker.Recycle();
				mVelocityTracker = null;
			}
		}

		//[Sharpen.OverridesMethod(@"ViewGroup")]
		public override void RequestDisallowInterceptTouchEvent(bool disallowIntercept)
		{
			if (disallowIntercept)
			{
				RecycleVelocityTracker();
			}
			base.RequestDisallowInterceptTouchEvent(disallowIntercept);
		}

		//[Sharpen.OverridesMethod(@"ViewGroup")]
		public override bool OnInterceptTouchEvent(MotionEvent ev)
		{
			var action = ev.Action;
			if ((action == MotionEventActions.Move) && (mIsBeingDragged))
			{
				return true;
			}
			switch (action & MotionEventActions.Mask)
			{
				case MotionEventActions.Move:
				{
					int activePointerId = mActivePointerId;
					if (activePointerId == INVALID_POINTER)
					{
						// If we don't have a valid id, the touch down wasn't on content.
						break;
					}
					int pointerIndex = ev.FindPointerIndex(activePointerId);
					float y = ev.GetY(pointerIndex);
					int yDiff = (int)System.Math.Abs(y - mLastMotionY);
					if (yDiff > mTouchSlop)
					{
						mIsBeingDragged = true;
						mLastMotionY = y;
						initVelocityTrackerIfNotExists();
						mVelocityTracker.AddMovement(ev);
						if (mScrollStrictSpan == null)
						{
							mScrollStrictSpan = StrictMode2.enterCriticalSpan("ScrollView-scroll");
						}
					}
					break;
				}

				case MotionEventActions.Down:
				{
					float y = ev.GetY();
					if (!inChild((int)ev.GetX(), (int)y))
					{
						mIsBeingDragged = false;
						RecycleVelocityTracker();
						break;
					}
					mLastMotionY = y;
					mActivePointerId = ev.GetPointerId(0);
					initOrResetVelocityTracker();
					mVelocityTracker.AddMovement(ev);
					mIsBeingDragged = !mScroller.isFinished();
					if (mIsBeingDragged && mScrollStrictSpan == null)
					{
						mScrollStrictSpan = StrictMode2.enterCriticalSpan("ScrollView-scroll");
					}
					break;
				}

				case MotionEventActions.Cancel:
				case MotionEventActions.Up:
				{
					mIsBeingDragged = false;
					mActivePointerId = INVALID_POINTER;
					RecycleVelocityTracker();
					if (mScroller.springBack(ScrollX, ScrollY, 0, 0, 0, getScrollRange()))
					{
						Invalidate();
					}
					break;
				}

				case MotionEventActions.PointerUp:
				{
					OnSecondaryPointerUp(ev);
					break;
				}
			}
			return mIsBeingDragged;
		}

		//[Sharpen.OverridesMethod(@"View")]
		public override bool OnTouchEvent(MotionEvent ev)
		{
			initVelocityTrackerIfNotExists();
			mVelocityTracker.AddMovement(ev);
			var action = ev.Action;
			switch (action & MotionEventActions.Mask)
			{
				case MotionEventActions.Down:
				{
					mIsBeingDragged = ChildCount != 0;
					if (!mIsBeingDragged)
					{
						return false;
					}
					if (!mScroller.isFinished())
					{
						mScroller.abortAnimation();
						if (mFlingStrictSpan != null)
						{
							mFlingStrictSpan.finish();
							mFlingStrictSpan = null;
						}
					}
					// Remember where the motion event started
					mLastMotionY = ev.GetY();
					mActivePointerId = ev.GetPointerId(0);
					break;
				}

				case MotionEventActions.Move:
				{
					if (mIsBeingDragged)
					{
						// Scroll to follow the motion event
						int activePointerIndex = ev.FindPointerIndex(mActivePointerId);
						float y = ev.GetY(activePointerIndex);
						int deltaY = (int)(mLastMotionY - y);
						mLastMotionY = y;
						int oldX = ScrollX;
						int oldY = ScrollY;
						int range = getScrollRange();
						var overscrollMode = OverScrollMode;
						bool canOverscroll = overscrollMode == OverScrollMode.Always || (overscrollMode == OverScrollMode.IfContentScrolls
							 && range > 0);
						if (OverScrollBy(0, deltaY, 0, ScrollY, 0, range, 0, mOverscrollDistance, true))
						{
							// Break our velocity if we hit a scroll barrier.
							mVelocityTracker.Clear();
						}
						OnScrollChanged(ScrollX, ScrollY, oldX, oldY);
						if (canOverscroll)
						{
							int pulledToY = oldY + deltaY;
							if (pulledToY < 0)
							{
								mEdgeGlowTop.OnPull((float)deltaY / Height);
								if (!mEdgeGlowBottom.IsFinished)
								{
									mEdgeGlowBottom.OnRelease();
								}
							}
							else
							{
								if (pulledToY > range)
								{
									mEdgeGlowBottom.OnPull((float)deltaY / Height);
									if (!mEdgeGlowTop.IsFinished)
									{
										mEdgeGlowTop.OnRelease();
									}
								}
							}
							if (mEdgeGlowTop != null && (!mEdgeGlowTop.IsFinished || !mEdgeGlowBottom.IsFinished
								))
							{
								Invalidate();
							}
						}
					}
					break;
				}

				case MotionEventActions.Up:
				{
					if (mIsBeingDragged)
					{
						VelocityTracker velocityTracker = mVelocityTracker;
						velocityTracker.ComputeCurrentVelocity(1000, mMaximumVelocity);
						int initialVelocity = (int)velocityTracker.GetYVelocity(mActivePointerId);
						if (ChildCount > 0)
						{
							if ((System.Math.Abs(initialVelocity) > mMinimumVelocity))
							{
								fling(-initialVelocity);
							}
							else
							{
								if (mScroller.springBack(ScrollX, ScrollY, 0, 0, 0, getScrollRange()))
								{
									Invalidate();
								}
							}
						}
						mActivePointerId = INVALID_POINTER;
						endDrag();
					}
					break;
				}

				case MotionEventActions.Cancel:
				{
					if (mIsBeingDragged && ChildCount > 0)
					{
						if (mScroller.springBack(ScrollX, ScrollY, 0, 0, 0, getScrollRange()))
						{
							Invalidate();
						}
						mActivePointerId = INVALID_POINTER;
						endDrag();
					}
					break;
				}

				case MotionEventActions.PointerDown:
				{
					int index = ev.ActionIndex;
					float y = ev.GetY(index);
					mLastMotionY = y;
					mActivePointerId = ev.GetPointerId(index);
					break;
				}

				case MotionEventActions.PointerUp:
				{
					OnSecondaryPointerUp(ev);
					mLastMotionY = ev.GetY(ev.FindPointerIndex(mActivePointerId));
					break;
				}
			}
			return true;
		}

		private void OnSecondaryPointerUp(MotionEvent ev)
		{
			var pointerIndex = ((Int32)ev.Action & (Int32)MotionEventActions.PointerIndexMask
				) >> (Int32)MotionEventActions.PointerIndexShift;
			int pointerId = ev.GetPointerId(pointerIndex);
			if (pointerId == mActivePointerId)
			{
				// This was our active pointer going up. Choose a new
				// active pointer and adjust accordingly.
				// TODO: Make this decision more intelligent.
				int newPointerIndex = pointerIndex == 0 ? 1 : 0;
				mLastMotionY = ev.GetY(newPointerIndex);
				mActivePointerId = ev.GetPointerId(newPointerIndex);
				if (mVelocityTracker != null)
				{
					mVelocityTracker.Clear();
				}
			}
		}


        private float mVerticalScrollFactor;

        protected float getVerticalScrollFactor()
        {
            if (mVerticalScrollFactor == 0)
            {
                TypedValue outValue = new TypedValue();
                if (!Context.Theme.ResolveAttribute(16842829,
                    outValue, true))
                {
                    throw new Exception(
                        "Expected theme to define listPreferredItemHeight.");
                }

                mVerticalScrollFactor = outValue.GetDimension(
                    Context.Resources.DisplayMetrics);
            }

            return mVerticalScrollFactor;
        }
    
		

		//[Sharpen.OverridesMethod(@"View")]
		public override bool OnGenericMotionEvent(MotionEvent @event)
		{
			if ((@event.Source & InputSourceType.ClassPointer) != 0)
			{
				switch (@event.Action)
				{
					case MotionEventActions.Scroll:
					{
						if (!mIsBeingDragged)
						{
							float vscroll = @event.GetAxisValue(Axis.Vscroll);
							if (vscroll != 0)
							{
								
									

								int delta = (int)(vscroll * getVerticalScrollFactor());
								int range = getScrollRange();
								int oldScrollY = ScrollY;
								int newScrollY = oldScrollY - delta;
								if (newScrollY < 0)
								{
									newScrollY = 0;
								}
								else
								{
									if (newScrollY > range)
									{
										newScrollY = range;
									}
								}
								if (newScrollY != oldScrollY)
								{
									base.ScrollTo(ScrollX, newScrollY);
									return true;
								}
							}
						}
						break;
					}
				}
			}
			return base.OnGenericMotionEvent(@event);
		}

		//[Sharpen.OverridesMethod(@"View")]
		protected override void OnOverScrolled(int scrollX, int scrollY, bool clampedX
			, bool clampedY)
		{
			// Treat animating scrolls differently; see #computeScroll() for why.
			if (!mScroller.isFinished())
			{
				ScrollX = scrollX;
				ScrollY = scrollY;
				
				

				invalidateParentIfNeeded();
				if (clampedY)
				{
					mScroller.springBack(ScrollX, ScrollY, 0, 0, 0, getScrollRange());
				}
			}
			else
			{
				base.ScrollTo(scrollX, scrollY);
			}
			AwakenScrollBars();
		}

        protected internal virtual void invalidateParentIfNeeded()
        {
            if (IsHardwareAccelerated && Parent is View)
            {
                ((View)Parent).Invalidate();
            }
        }

		//[Sharpen.OverridesMethod(@"View")]
		public override void OnInitializeAccessibilityNodeInfo(AccessibilityNodeInfo
			 info)
		{
			base.OnInitializeAccessibilityNodeInfo(info);
			info.Scrollable = getScrollRange() > 0;
		}

		//[Sharpen.OverridesMethod(@"View")]
		public override void OnInitializeAccessibilityEvent(AccessibilityEvent
			 @event)
		{
			base.OnInitializeAccessibilityEvent(@event);
			bool scrollable = getScrollRange() > 0;
			@event.Scrollable = scrollable;
			@event.ScrollX = (ScrollX);
			@event.ScrollY = (ScrollY);
			@event.MaxScrollX = (ScrollX);
			@event.MaxScrollY = (getScrollRange());
		}

		private int getScrollRange()
		{
			int scrollRange = 0;
			if (ChildCount > 0)
			{
				View child = GetChildAt(0);
				scrollRange = System.Math.Max(0, child.Height - (Height - PaddingBottom
					 - PaddingTop));
			}
			return scrollRange;
		}

		/// <summary>
		/// <p>
		/// Finds the next focusable component that fits in this View's bounds
		/// (excluding fading edges) pretending that this View's top is located at
		/// the parameter top.
		/// </summary>
		/// <remarks>
		/// <p>
		/// Finds the next focusable component that fits in this View's bounds
		/// (excluding fading edges) pretending that this View's top is located at
		/// the parameter top.
		/// </p>
		/// </remarks>
		/// <param name="topFocus">
		/// look for a candidate at the top of the bounds if topFocus is true,
		/// or at the bottom of the bounds if topFocus is false
		/// </param>
		/// <param name="top">
		/// the top offset of the bounds in which a focusable must be
		/// found (the fading edge is assumed to start at this position)
		/// </param>
		/// <param name="preferredFocusable">
		/// the View that has highest priority and will be
		/// returned if it is within my bounds (null is valid)
		/// </param>
		/// <returns>the next focusable component in the bounds or null if none can be found</returns>
		private View findFocusableViewInMyBounds(bool topFocus, int top, View
			 preferredFocusable)
		{
			int fadingEdgeLength = VerticalFadingEdgeLength / 2;
			int topWithoutFadingEdge = top + fadingEdgeLength;
			int bottomWithoutFadingEdge = top + Height - fadingEdgeLength;
			if ((preferredFocusable != null) && (preferredFocusable.Top < bottomWithoutFadingEdge
				) && (preferredFocusable.Bottom > topWithoutFadingEdge))
			{
				return preferredFocusable;
			}
			return findFocusableViewInBounds(topFocus, topWithoutFadingEdge, bottomWithoutFadingEdge
				);
		}

		/// <summary>
		/// <p>
		/// Finds the next focusable component that fits in the specified bounds.
		/// </summary>
		/// <remarks>
		/// <p>
		/// Finds the next focusable component that fits in the specified bounds.
		/// </p>
		/// </remarks>
		/// <param name="topFocus">
		/// look for a candidate is the one at the top of the bounds
		/// if topFocus is true, or at the bottom of the bounds if topFocus is
		/// false
		/// </param>
		/// <param name="top">
		/// the top offset of the bounds in which a focusable must be
		/// found
		/// </param>
		/// <param name="bottom">
		/// the bottom offset of the bounds in which a focusable must
		/// be found
		/// </param>
		/// <returns>
		/// the next focusable component in the bounds or null if none can
		/// be found
		/// </returns>
		private View findFocusableViewInBounds(bool topFocus, int top, int bottom
			)
		{
			var focusables = GetFocusables(FocusSearchDirection.Forward
				);
			View focusCandidate = null;
			bool foundFullyContainedFocusable = false;
			int count = focusables.Count;
			{
				for (int i = 0; i < count; i++)
				{
					View view = focusables[i];
					int viewTop = view.Top;
					int viewBottom = view.Bottom;
					if (top < viewBottom && viewTop < bottom)
					{
						bool viewIsFullyContained = (top < viewTop) && (viewBottom < bottom);
						if (focusCandidate == null)
						{
							focusCandidate = view;
							foundFullyContainedFocusable = viewIsFullyContained;
						}
						else
						{
							bool viewIsCloserToBoundary = (topFocus && viewTop < focusCandidate.Top) || 
								(!topFocus && viewBottom > focusCandidate.Bottom);
							if (foundFullyContainedFocusable)
							{
								if (viewIsFullyContained && viewIsCloserToBoundary)
								{
									focusCandidate = view;
								}
							}
							else
							{
								if (viewIsFullyContained)
								{
									focusCandidate = view;
									foundFullyContainedFocusable = true;
								}
								else
								{
									if (viewIsCloserToBoundary)
									{
										focusCandidate = view;
									}
								}
							}
						}
					}
				}
			}
			return focusCandidate;
		}

		/// <summary><p>Handles scrolling in response to a "page up/down" shortcut press.</summary>
		/// <remarks>
		/// <p>Handles scrolling in response to a "page up/down" shortcut press. This
		/// method will scroll the view by one page up or down and give the focus
		/// to the topmost/bottommost component in the new visible area. If no
		/// component is a good candidate for focus, this scrollview reclaims the
		/// focus.</p>
		/// </remarks>
		/// <param name="direction">
		/// the scroll direction:
		/// <see cref="0x00000021">0x00000021</see>
		/// to go one page up or
		/// <see cref="0x00000082">0x00000082</see>
		/// to go one page down
		/// </param>
		/// <returns>true if the key event is consumed by this method, false otherwise</returns>
		public virtual bool pageScroll(int direction)
		{
			bool down = direction == 0x00000082;
			int height = Height;
			if (down)
			{
				mTempRect.Top = ScrollY + height;
				int count = ChildCount;
				if (count > 0)
				{
					View view = GetChildAt(count - 1);
					if (mTempRect.Top + height > view.Bottom)
					{
						mTempRect.Top = view.Bottom - height;
					}
				}
			}
			else
			{
				mTempRect.Top = ScrollY - height;
				if (mTempRect.Top < 0)
				{
					mTempRect.Top = 0;
				}
			}
			mTempRect.Bottom = mTempRect.Top + height;
			return scrollAndFocus(direction, mTempRect.Top, mTempRect.Bottom);
		}

		/// <summary><p>Handles scrolling in response to a "home/end" shortcut press.</summary>
		/// <remarks>
		/// <p>Handles scrolling in response to a "home/end" shortcut press. This
		/// method will scroll the view to the top or bottom and give the focus
		/// to the topmost/bottommost component in the new visible area. If no
		/// component is a good candidate for focus, this scrollview reclaims the
		/// focus.</p>
		/// </remarks>
		/// <param name="direction">
		/// the scroll direction:
		/// <see cref="0x00000021">0x00000021</see>
		/// to go the top of the view or
		/// <see cref="0x00000082">0x00000082</see>
		/// to go the bottom
		/// </param>
		/// <returns>true if the key event is consumed by this method, false otherwise</returns>
		public virtual bool fullScroll(int direction)
		{
			bool down = direction == 0x00000082;
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

		/// <summary>
		/// <p>Scrolls the view to make the area defined by <code>top</code> and
		/// <code>bottom</code> visible.
		/// </summary>
		/// <remarks>
		/// <p>Scrolls the view to make the area defined by <code>top</code> and
		/// <code>bottom</code> visible. This method attempts to give the focus
		/// to a component visible in this area. If no component can be focused in
		/// the new visible area, the focus is reclaimed by this ScrollView.</p>
		/// </remarks>
		/// <param name="direction">
		/// the scroll direction:
		/// <see cref="0x00000021">0x00000021</see>
		/// to go upward,
		/// <see cref="0x00000082">0x00000082</see>
		/// to downward
		/// </param>
		/// <param name="top">the top offset of the new area to be made visible</param>
		/// <param name="bottom">the bottom offset of the new area to be made visible</param>
		/// <returns>true if the key event is consumed by this method, false otherwise</returns>
		private bool scrollAndFocus(int direction, int top, int bottom)
		{
			bool handled = true;
			int height = Height;
			int containerTop = ScrollY;
			int containerBottom = containerTop + height;
			bool up = direction == 0x00000021;
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
			if (newFocused != FindFocus())
			{
				newFocused.RequestFocus((FocusSearchDirection)direction);
			}
			return handled;
		}

		/// <summary>Handle scrolling in response to an up or down arrow click.</summary>
		/// <remarks>Handle scrolling in response to an up or down arrow click.</remarks>
		/// <param name="direction">
		/// The direction corresponding to the arrow key that was
		/// pressed
		/// </param>
		/// <returns>True if we consumed the event, false otherwise</returns>
		public virtual bool arrowScroll(int direction)
		{
			View currentFocused = FindFocus();
			if (currentFocused == this)
			{
				currentFocused = null;
			}
			View nextFocused = FocusFinder.Instance.FindNextFocus
				(this, currentFocused, (FocusSearchDirection) direction);
			int maxJump = getMaxScrollAmount();
			if (nextFocused != null && isWithinDeltaOfScreen(nextFocused, maxJump, Height
				))
			{
				nextFocused.GetDrawingRect(mTempRect);
				OffsetDescendantRectToMyCoords(nextFocused, mTempRect);
				int scrollDelta = computeScrollDeltaToGetChildRectOnScreen(mTempRect);
				doScrollY(scrollDelta);
				nextFocused.RequestFocus((FocusSearchDirection)direction);
			}
			else
			{
				// no new focus
				int scrollDelta = maxJump;
				if (direction == 0x00000021 && ScrollY < scrollDelta)
				{
					scrollDelta = ScrollY;
				}
				else
				{
					if (direction == 0x00000082)
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
				}
				if (scrollDelta == 0)
				{
					return false;
				}
				doScrollY(direction == 0x00000082 ? scrollDelta : -scrollDelta);
			}
			if (currentFocused != null && currentFocused.IsFocused && isOffScreen(currentFocused
				))
			{
				// previously focused item still has focus and is off screen, give
				// it up (take it back to ourselves)
				// (also, need to temporarily force FOCUS_BEFORE_DESCENDANTS so we are
				// sure to
				// get it)
				var descendantFocusability = DescendantFocusability;
				// save
                DescendantFocusability = DescendantFocusability.BeforeDescendants;
				
				RequestFocus();
                DescendantFocusability = descendantFocusability;
				
			}
			// restore
			return true;
		}

		/// <returns>
		/// whether the descendant of this scroll view is scrolled off
		/// screen.
		/// </returns>
		private bool isOffScreen(View descendant)
		{
			return !isWithinDeltaOfScreen(descendant, 0, Height);
		}

		/// <returns>
		/// whether the descendant of this scroll view is within delta
		/// pixels of being on the screen.
		/// </returns>
		private bool isWithinDeltaOfScreen(View descendant, int delta, int height
			)
		{
			descendant.GetDrawingRect(mTempRect);
			OffsetDescendantRectToMyCoords(descendant, mTempRect);
			return (mTempRect.Bottom + delta) >= ScrollY && (mTempRect.Top - delta) <= (
				ScrollY + height);
		}

		/// <summary>Smooth scroll by a Y delta</summary>
		/// <param name="delta">the number of pixels to scroll by on the Y axis</param>
		private void doScrollY(int delta)
		{
			if (delta != 0)
			{
				if (mSmoothScrollingEnabled)
				{
					smoothScrollBy(0, delta);
				}
				else
				{
					ScrollBy(0, delta);
				}
			}
		}

		/// <summary>
		/// Like
		/// <see cref="View.ScrollBy(int, int)">View.ScrollBy(int, int)
		/// 	</see>
		/// , but scroll smoothly instead of immediately.
		/// </summary>
		/// <param name="dx">the number of pixels to scroll by on the X axis</param>
		/// <param name="dy">the number of pixels to scroll by on the Y axis</param>
		public void smoothScrollBy(int dx, int dy)
		{
			if (ChildCount == 0)
			{
				// Nothing to do.
				return;
			}
			long duration = AnimationUtils.CurrentAnimationTimeMillis(
				) - mLastScroll;
			if (duration > ANIMATED_SCROLL_GAP)
			{
				int height = Height - PaddingBottom - PaddingTop;
				int bottom = GetChildAt(0).Height;
				int maxY = System.Math.Max(0, bottom - height);
				int scrollY = ScrollY;
				dy = System.Math.Max(0, System.Math.Min(scrollY + dy, maxY)) - scrollY;
				mScroller.StartScroll(ScrollX, scrollY, 0, dy);
				Invalidate();
			}
			else
			{
				if (!mScroller.isFinished())
				{
					mScroller.abortAnimation();
					if (mFlingStrictSpan != null)
					{
						mFlingStrictSpan.finish();
						mFlingStrictSpan = null;
					}
				}
				ScrollBy(dx, dy);
			}
			mLastScroll = AnimationUtils.CurrentAnimationTimeMillis();
		}

		/// <summary>
		/// Like
		/// <see cref="scrollTo(int, int)">scrollTo(int, int)</see>
		/// , but scroll smoothly instead of immediately.
		/// </summary>
		/// <param name="x">the position where to scroll on the X axis</param>
		/// <param name="y">the position where to scroll on the Y axis</param>
		public void smoothScrollTo(int x, int y)
		{
			smoothScrollBy(x - ScrollX, y - ScrollY);
		}

		/// <summary>
		/// <p>The scroll range of a scroll view is the overall height of all of its
		/// children.</p>
		/// </summary>
		//[Sharpen.OverridesMethod(@"View")]
		protected override int ComputeVerticalScrollRange()
		{
			int count = ChildCount;
			int contentHeight = Height - PaddingBottom - PaddingTop;
			if (count == 0)
			{
				return contentHeight;
			}
			int scrollRange = GetChildAt(0).Bottom;
			int scrollY = ScrollY;
			int overscrollBottom = System.Math.Max(0, scrollRange - contentHeight);
			if (scrollY < 0)
			{
				scrollRange -= scrollY;
			}
			else
			{
				if (scrollY > overscrollBottom)
				{
					scrollRange += scrollY - overscrollBottom;
				}
			}
			return scrollRange;
		}

		//[Sharpen.OverridesMethod(@"View")]
		protected override int ComputeVerticalScrollOffset()
		{
			return System.Math.Max(0, base.ComputeVerticalScrollOffset());
		}

		//[Sharpen.OverridesMethod(@"ViewGroup")]
		protected override void MeasureChild(View child, int parentWidthMeasureSpec
			, int parentHeightMeasureSpec)
		{
			ViewGroup.LayoutParams lp = child.LayoutParameters;
			int childWidthMeasureSpec;
			int childHeightMeasureSpec;
			childWidthMeasureSpec = GetChildMeasureSpec(parentWidthMeasureSpec, PaddingLeft 
				+ PaddingRight, lp.Width);
			childHeightMeasureSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified
				);
			child.Measure(childWidthMeasureSpec, childHeightMeasureSpec);
		}

		//[Sharpen.OverridesMethod(@"ViewGroup")]
		protected override void MeasureChildWithMargins(View child, 
			int parentWidthMeasureSpec, int widthUsed, int parentHeightMeasureSpec, int heightUsed
			)
		{
			ViewGroup.MarginLayoutParams lp = (ViewGroup.MarginLayoutParams
				)child.LayoutParameters;
			int childWidthMeasureSpec = GetChildMeasureSpec(parentWidthMeasureSpec, PaddingLeft
				 + PaddingRight + lp.LeftMargin + lp.RightMargin + widthUsed, lp.Width);
			int childHeightMeasureSpec = View.MeasureSpec.MakeMeasureSpec(lp.TopMargin
				 + lp.BottomMargin, MeasureSpecMode.Unspecified);
			child.Measure(childWidthMeasureSpec, childHeightMeasureSpec);
		}

		//[Sharpen.OverridesMethod(@"View")]
		public override void ComputeScroll()
		{
			if (mScroller.computeScrollOffset())
			{
				// This is called at drawing time by ViewGroup.  We don't want to
				// re-show the scrollbars at this point, which scrollTo will do,
				// so we replicate most of scrollTo here.
				//
				//         It's a little odd to call onScrollChanged from inside the drawing.
				//
				//         It is, except when you remember that computeScroll() is used to
				//         animate scrolling. So unless we want to defer the onScrollChanged()
				//         until the end of the animated scrolling, we don't really have a
				//         choice here.
				//
				//         I agree.  The alternative, which I think would be worse, is to post
				//         something and tell the subclasses later.  This is bad because there
				//         will be a window where ScrollX/Y is different from what the app
				//         thinks it is.
				//
				int oldX = ScrollX;
				int oldY = ScrollY;
				int x = mScroller.getCurrX();
                int y = mScroller.getCurrY();
				if (oldX != x || oldY != y)
				{
					int range = getScrollRange();
					var overscrollMode = OverScrollMode;
					bool canOverscroll = overscrollMode == OverScrollMode.Always || (overscrollMode == OverScrollMode.IfContentScrolls 
						 && range > 0);
					OverScrollBy(x - oldX, y - oldY, oldX, oldY, 0, range, 0, mOverflingDistance, false
						);
					OnScrollChanged(ScrollX, ScrollY, oldX, oldY);
					if (canOverscroll)
					{
						if (y < 0 && oldY >= 0)
						{
							mEdgeGlowTop.OnAbsorb((int)mScroller.getCurrVelocity());
						}
						else
						{
							if (y > range && oldY <= range)
							{
								mEdgeGlowBottom.OnAbsorb((int)mScroller.getCurrVelocity());
							}
						}
					}
				}
				AwakenScrollBars();
				// Keep on drawing until the animation has finished.
				PostInvalidate();
			}
			else
			{
				if (mFlingStrictSpan != null)
				{
					mFlingStrictSpan.finish();
					mFlingStrictSpan = null;
				}
			}
		}

		/// <summary>Scrolls the view to the given child.</summary>
		/// <remarks>Scrolls the view to the given child.</remarks>
		/// <param name="child">the View to scroll to</param>
		private void scrollToChild(View child)
		{
			child.GetDrawingRect(mTempRect);
			OffsetDescendantRectToMyCoords(child, mTempRect);
			int scrollDelta = computeScrollDeltaToGetChildRectOnScreen(mTempRect);
			if (scrollDelta != 0)
			{
				ScrollBy(0, scrollDelta);
			}
		}

		/// <summary>
		/// If rect is off screen, scroll just enough to get it (or at least the
		/// first screen size chunk of it) on screen.
		/// </summary>
		/// <remarks>
		/// If rect is off screen, scroll just enough to get it (or at least the
		/// first screen size chunk of it) on screen.
		/// </remarks>
		/// <param name="rect">The rectangle.</param>
		/// <param name="immediate">True to scroll immediately without animation</param>
		/// <returns>true if scrolling was performed</returns>
		private bool scrollToChildRect(Rect rect, bool immediate)
		{
			int delta = computeScrollDeltaToGetChildRectOnScreen(rect);
			bool scroll = delta != 0;
			if (scroll)
			{
				if (immediate)
				{
					ScrollBy(0, delta);
				}
				else
				{
					smoothScrollBy(0, delta);
				}
			}
			return scroll;
		}

		/// <summary>
		/// Compute the amount to scroll in the Y direction in order to get
		/// a rectangle completely on the screen (or, if taller than the screen,
		/// at least the first screen size chunk of it).
		/// </summary>
		/// <remarks>
		/// Compute the amount to scroll in the Y direction in order to get
		/// a rectangle completely on the screen (or, if taller than the screen,
		/// at least the first screen size chunk of it).
		/// </remarks>
		/// <param name="rect">The rect.</param>
		/// <returns>The scroll delta.</returns>
		protected internal virtual int computeScrollDeltaToGetChildRectOnScreen(Rect
			 rect)
		{
			if (ChildCount == 0)
			{
				return 0;
			}
			int height = Height;
			int screenTop = ScrollY;
			int screenBottom = screenTop + height;
			int fadingEdge = VerticalFadingEdgeLength;
			// leave room for top fading edge as long as rect isn't at very top
			if (rect.Top > 0)
			{
				screenTop += fadingEdge;
			}
			// leave room for bottom fading edge as long as rect isn't at very bottom
			if (rect.Bottom < GetChildAt(0).Height)
			{
				screenBottom -= fadingEdge;
			}
			int scrollYDelta = 0;
			if (rect.Bottom > screenBottom && rect.Top > screenTop)
			{
				// need to move down to get it in view: move down just enough so
				// that the entire rectangle is in view (or at least the first
				// screen size chunk).
				if (rect.Height() > height)
				{
					// just enough to get screen size chunk on
					scrollYDelta += (rect.Top - screenTop);
				}
				else
				{
					// get entire rect at bottom of screen
					scrollYDelta += (rect.Bottom - screenBottom);
				}
				// make sure we aren't scrolling beyond the end of our content
				int bottom = GetChildAt(0).Bottom;
				int distanceToBottom = bottom - screenBottom;
				scrollYDelta = System.Math.Min(scrollYDelta, distanceToBottom);
			}
			else
			{
				if (rect.Top < screenTop && rect.Bottom < screenBottom)
				{
					// need to move up to get it in view: move up just enough so that
					// entire rectangle is in view (or at least the first screen
					// size chunk of it).
					if (rect.Height() > height)
					{
						// screen size chunk
						scrollYDelta -= (screenBottom - rect.Bottom);
					}
					else
					{
						// entire rect at top
						scrollYDelta -= (screenTop - rect.Top);
					}
					// make sure we aren't scrolling any further than the top our content
					scrollYDelta = System.Math.Max(scrollYDelta, -ScrollY);
				}
			}
			return scrollYDelta;
		}

		//[Sharpen.OverridesMethod(@"ViewGroup")]
		public override void RequestChildFocus(View child, View
			 focused)
		{
			if (!mIsLayoutDirty)
			{
				scrollToChild(focused);
			}
			else
			{
				// The child may not be laid out yet, we can't compute the scroll yet
				mChildToScrollTo = focused;
			}
			base.RequestChildFocus(child, focused);
		}

		/// <summary>
		/// When looking for focus in children of a scroll view, need to be a little
		/// more careful not to give focus to something that is scrolled off screen.
		/// </summary>
		/// <remarks>
		/// When looking for focus in children of a scroll view, need to be a little
		/// more careful not to give focus to something that is scrolled off screen.
		/// This is more expensive than the default
		/// <see cref="ViewGroup">ViewGroup</see>
		/// implementation, otherwise this behavior might have been made the default.
		/// </remarks>
		//[Sharpen.OverridesMethod(@"ViewGroup")]
		protected override bool OnRequestFocusInDescendants(int direction, Rect
			 previouslyFocusedRect)
		{
			// convert from forward / backward notation to up / down / left / right
			// (ugh).
			if (direction == 0x00000002)
			{
				direction = 0x00000082;
			}
			else
			{
				if (direction == 0x00000001)
				{
					direction = 0x00000021;
				}
			}
			View nextFocus = previouslyFocusedRect == null ? FocusFinder
				.Instance.FindNextFocus(this, null, (FocusSearchDirection)direction) 
                : FocusFinder.Instance
				.FindNextFocusFromRect(this, previouslyFocusedRect, (FocusSearchDirection)direction);
			if (nextFocus == null)
			{
				return false;
			}
			if (isOffScreen(nextFocus))
			{
				return false;
			}
			return nextFocus.RequestFocus((FocusSearchDirection)direction, previouslyFocusedRect);
		}

		//[Sharpen.OverridesMethod(@"ViewGroup")]
		public override bool RequestChildRectangleOnScreen(View child, Rect
			 rectangle, bool immediate)
		{
			// offset into coordinate space of this scroll view
			rectangle.Offset(child.Left - child.ScrollX, child.Top - child.ScrollY
				);
			return scrollToChildRect(rectangle, immediate);
		}

		//[Sharpen.OverridesMethod(@"View")]
		public override void RequestLayout()
		{
			mIsLayoutDirty = true;
			base.RequestLayout();
		}

		//[Sharpen.OverridesMethod(@"View")]
		protected override void OnDetachedFromWindow()
		{
			base.OnDetachedFromWindow();
			if (mScrollStrictSpan != null)
			{
				mScrollStrictSpan.finish();
				mScrollStrictSpan = null;
			}
			if (mFlingStrictSpan != null)
			{
				mFlingStrictSpan.finish();
				mFlingStrictSpan = null;
			}
		}

		//[Sharpen.OverridesMethod(@"View")]
		protected override void OnLayout(bool changed, int l, int t, int r, int 
			b)
		{
			base.OnLayout(changed, l, t, r, b);
			mIsLayoutDirty = false;
			// Give a child focus if it needs it
			if (mChildToScrollTo != null && isViewDescendantOf(mChildToScrollTo, this))
			{
				scrollToChild(mChildToScrollTo);
			}
			mChildToScrollTo = null;
			// Calling this with the present values causes it to re-clam them
			ScrollTo(ScrollX, ScrollY);
		}

		//[Sharpen.OverridesMethod(@"View")]
		protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
		{
			base.OnSizeChanged(w, h, oldw, oldh);
			View currentFocused = FindFocus();
			if (null == currentFocused || this == currentFocused)
			{
				return;
			}
			// If the currently-focused view was visible on the screen when the
			// screen was at the old height, then scroll the screen to make that
			// view visible with the new screen height.
			if (isWithinDeltaOfScreen(currentFocused, 0, oldh))
			{
				currentFocused.GetDrawingRect(mTempRect);
				OffsetDescendantRectToMyCoords(currentFocused, mTempRect);
				int scrollDelta = computeScrollDeltaToGetChildRectOnScreen(mTempRect);
				doScrollY(scrollDelta);
			}
		}

		/// <summary>Return true if child is an descendant of parent, (or equal to the parent).
		/// 	</summary>
		/// <remarks>Return true if child is an descendant of parent, (or equal to the parent).
		/// 	</remarks>
		private bool isViewDescendantOf(View child, View parent
			)
		{
			if (child == parent)
			{
				return true;
			}
			var theParent = child.Parent;
			return (theParent is ViewGroup) && isViewDescendantOf((View
				)theParent, parent);
		}

		/// <summary>Fling the scroll view</summary>
		/// <param name="velocityY">
		/// The initial velocity in the Y direction. Positive
		/// numbers mean that the finger/cursor is moving down the screen,
		/// which means we want to scroll towards the top.
		/// </param>
		public virtual void fling(int velocityY)
		{
			if (ChildCount > 0)
			{
				int height = Height - PaddingBottom - PaddingTop;
				int bottom = GetChildAt(0).Height;
				mScroller.fling(ScrollX, ScrollY, 0, velocityY, 0, 0, 0, System.Math.Max(0, bottom
					 - height), 0, height / 2);
				bool movingDown = velocityY > 0;
				if (mFlingStrictSpan == null)
				{
					mFlingStrictSpan = StrictMode2.enterCriticalSpan("ScrollView-fling");
				}
				Invalidate();
			}
		}

		private void endDrag()
		{
			mIsBeingDragged = false;
			RecycleVelocityTracker();
			if (mEdgeGlowTop != null)
			{
				mEdgeGlowTop.OnRelease();
				mEdgeGlowBottom.OnRelease();
			}
			if (mScrollStrictSpan != null)
			{
				mScrollStrictSpan.finish();
				mScrollStrictSpan = null;
			}
		}

		/// <summary>
		/// <inheritDoc></inheritDoc>
		/// <p>This version also clamps the scrolling to the bounds of our child.
		/// </summary>
		//[Sharpen.OverridesMethod(@"View")]
		public override void ScrollTo(int x, int y)
		{
			// we rely on the fact the View.ScrollBy calls scrollTo.
			if (ChildCount > 0)
			{
				View child = GetChildAt(0);
				x = clamp(x, Width - PaddingRight - PaddingLeft, child.Width);
				y = clamp(y, Height - PaddingBottom - PaddingTop, child.Height);
				if (x != ScrollX || y != ScrollY)
				{
					base.ScrollTo(x, y);
				}
			}
		}

        public override OverScrollMode OverScrollMode
        {
            get => base.OverScrollMode;
			set
            {
                if (value != OverScrollMode.Never)
                {
                    if (mEdgeGlowTop == null)
                    {
                        Context context = Context;
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

       

		//[Sharpen.OverridesMethod(@"View")]
		public override void Draw(Canvas canvas)
		{
			base.Draw(canvas);
			if (mEdgeGlowTop != null)
			{
				int scrollY = ScrollY;
				if (!mEdgeGlowTop.IsFinished)
				{
					int restoreCount = canvas.Save();
					int width = Width - PaddingLeft - PaddingRight;
					canvas.Translate(PaddingLeft, System.Math.Min(0, scrollY));
					mEdgeGlowTop.SetSize(width, Height);
					if (mEdgeGlowTop.Draw(canvas))
					{
						Invalidate();
					}
					canvas.RestoreToCount(restoreCount);
				}
				if (!mEdgeGlowBottom.IsFinished)
				{
					int restoreCount = canvas.Save();
					int width = Width - PaddingLeft - PaddingRight;
					int height = Height;
					canvas.Translate(-width + PaddingLeft, System.Math.Max(getScrollRange(), scrollY
						) + height);
					canvas.Rotate(180, width, 0);
					mEdgeGlowBottom.SetSize(width, height);
					if (mEdgeGlowBottom.Draw(canvas))
					{
						Invalidate();
					}
					canvas.RestoreToCount(restoreCount);
				}
			}
		}

		private int clamp(int n, int my, int child)
		{
			if (my >= child || n < 0)
			{
				return 0;
			}
			if ((my + n) > child)
			{
				return child - my;
			}
			return n;
		}
	}
}
