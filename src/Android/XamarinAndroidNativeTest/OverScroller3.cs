using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Hardware;
using Android.Util;
using Android.Views;
using Android.Views.Animations;

namespace XamarinAndroidNativeTest
{
    public class OverScroller3 : IOverScroller
    {
        static OverScroller3()
        {
            sViscousFluidScale = 8.0f;
            // must be set to 1.0 (used in viscousFluid())
            sViscousFluidNormalize = 1.0f;
            sViscousFluidNormalize = 1.0f / viscousFluid(1.0f);
        }

        /// <summary>
        ///     Creates an OverScroller with a viscous fluid scroll interpolator and flywheel.
        /// </summary>
        /// <remarks>
        ///     Creates an OverScroller with a viscous fluid scroll interpolator and flywheel.
        /// </remarks>
        /// <param name="context"></param>
        public OverScroller3(Context context) : this(context, null)
        {
        }

        /// <summary>Creates an OverScroller with flywheel enabled.</summary>
        /// <remarks>Creates an OverScroller with flywheel enabled.</remarks>
        /// <param name="context">The context of this application.</param>
        /// <param name="interpolator">
        ///     The scroll interpolator. If null, a default (viscous) interpolator will
        ///     be used.
        /// </param>
        public OverScroller3(Context context,
                             Interpolator
                                 interpolator) : this(context, interpolator, true)
        {
        }

        /// <summary>Creates an OverScroller.</summary>
        /// <remarks>Creates an OverScroller.</remarks>
        /// <param name="context">The context of this application.</param>
        /// <param name="interpolator">
        ///     The scroll interpolator. If null, a default (viscous) interpolator will
        ///     be used.
        /// </param>
        /// <param name="flywheel">
        ///     If true, successive fling motions will keep on increasing scroll speed.
        /// </param>
        /// <hide></hide>
        public OverScroller3(Context context,
                             Interpolator
                                 interpolator,
                             Boolean flywheel)
        {
            mInterpolator = interpolator;
            mFlywheel = flywheel;
            mScrollerX = new SplineOverScroller();
            mScrollerY = new SplineOverScroller();
            SplineOverScroller.initFromContext(context);
        }

        /// <summary>Creates an OverScroller with flywheel enabled.</summary>
        /// <remarks>Creates an OverScroller with flywheel enabled.</remarks>
        /// <param name="context">The context of this application.</param>
        /// <param name="interpolator">
        ///     The scroll interpolator. If null, a default (viscous) interpolator will
        ///     be used.
        /// </param>
        /// <param name="bounceCoefficientX">
        ///     A value between 0 and 1 that will determine the proportion of the
        ///     velocity which is preserved in the bounce when the horizontal edge is reached. A null value
        ///     means no bounce. This behavior is no longer supported and this coefficient has no effect.
        /// </param>
        /// <param name="bounceCoefficientY">
        ///     Same as bounceCoefficientX but for the vertical direction. This
        ///     behavior is no longer supported and this coefficient has no effect.
        ///     !deprecated Use {!link #OverScroller(Context, Interpolator, boolean)} instead.
        /// </param>
        public OverScroller3(Context context,
                             Interpolator
                                 interpolator,
                             Single bounceCoefficientX,
                             Single bounceCoefficientY) : this(context
            , interpolator, true)
        {
        }

        /// <summary>Creates an OverScroller.</summary>
        /// <remarks>Creates an OverScroller.</remarks>
        /// <param name="context">The context of this application.</param>
        /// <param name="interpolator">
        ///     The scroll interpolator. If null, a default (viscous) interpolator will
        ///     be used.
        /// </param>
        /// <param name="bounceCoefficientX">
        ///     A value between 0 and 1 that will determine the proportion of the
        ///     velocity which is preserved in the bounce when the horizontal edge is reached. A null value
        ///     means no bounce. This behavior is no longer supported and this coefficient has no effect.
        /// </param>
        /// <param name="bounceCoefficientY">
        ///     Same as bounceCoefficientX but for the vertical direction. This
        ///     behavior is no longer supported and this coefficient has no effect.
        /// </param>
        /// <param name="flywheel">
        ///     If true, successive fling motions will keep on increasing scroll speed.
        ///     !deprecated Use {!link OverScroller(Context, Interpolator, boolean)} instead.
        /// </param>
        public OverScroller3(Context context,
                             Interpolator
                                 interpolator,
                             Single bounceCoefficientX,
                             Single bounceCoefficientY,
                             Boolean flywheel
        ) : this(context, interpolator, flywheel)
        {
        }

        public Single CurrentYVelocity => mScrollerY.mCurrVelocity;

        public Single CurrVelocity => getCurrVelocity();

        public Single CurrY => getCurrY();

        public Int32 FinalY => getFinalY();

        public Int32 StartY => getStartY();

        /// <summary>Stops the animation.</summary>
        /// <remarks>
        ///     Stops the animation. Contrary to
        ///     <see cref="forceFinished(bool)">forceFinished(bool)</see>
        ///     ,
        ///     aborting the animating causes the scroller to move to the final x and y
        ///     positions.
        /// </remarks>
        /// <seealso cref="forceFinished(bool)">forceFinished(bool)</seealso>
        public virtual void abortAnimation()
        {
            mScrollerX.finish();
            mScrollerY.finish();
        }

        /// <summary>Call this when you want to know the new location.</summary>
        /// <remarks>
        ///     Call this when you want to know the new location. If it returns true, the
        ///     animation is not yet finished.
        /// </remarks>
        public virtual Boolean computeScrollOffset()
        {
            if (isFinished())
                return false;
            switch (mMode)
            {
                case SCROLL_MODE:
                {
                    var time = AnimationUtils.CurrentAnimationTimeMillis();
                    // Any scroller can be used for time, since they were started
                    // together in scroll mode. We use X here.
                    var elapsedTime = time - mScrollerX.mStartTime;
                    var duration = mScrollerX.mDuration;
                    if (elapsedTime < duration)
                    {
                        var q = (Single) elapsedTime / duration;
                        if (mInterpolator == null)
                            q = viscousFluid(q);
                        //else
                        //    q = mInterpolator.getInterpolation(q);
                        mScrollerX.updateScroll(q);
                        mScrollerY.updateScroll(q);
                    }
                    else
                        abortAnimation();

                    break;
                }

                case FLING_MODE:
                {
                    if (!mScrollerX.mFinished)
                        if (!mScrollerX.update())
                            if (!mScrollerX.continueWhenFinished())
                                mScrollerX.finish();
                    if (!mScrollerY.mFinished)
                        if (!mScrollerY.update())
                            if (!mScrollerY.continueWhenFinished())
                                mScrollerY.finish();
                    break;
                }
            }

            return true;
        }

        /// <summary>Extend the scroll animation.</summary>
        /// <remarks>
        ///     Extend the scroll animation. This allows a running animation to scroll
        ///     further and longer, when used with
        ///     <see cref="setFinalX(int)">setFinalX(int)</see>
        ///     or
        ///     <see cref="setFinalY(int)">setFinalY(int)</see>
        ///     .
        /// </remarks>
        /// <param name="extend">Additional time to scroll in milliseconds.</param>
        /// <seealso cref="setFinalX(int)">setFinalX(int)</seealso>
        /// <seealso cref="setFinalY(int)">setFinalY(int)</seealso>
        /// <hide>Pending removal once nothing depends on it</hide>
        [ObsoleteAttribute(
            @"OverScrollers don't necessarily have a fixed duration. Instead of setting a new final position and extending the duration of an existing scroll, use startScroll to begin a new animation."
        )]
        public virtual void extendDuration(Int32 extend)
        {
            mScrollerX.extendDuration(extend);
            mScrollerY.extendDuration(extend);
        }

        public virtual void fling(Int32 startX,
                                  Int32 startY,
                                  Int32 velocityX,
                                  Int32 velocityY,
                                  Int32
                                      minX,
                                  Int32 maxX,
                                  Int32 minY,
                                  Int32 maxY)
        {
            fling(startX, startY, velocityX, velocityY, minX, maxX, minY, maxY, 0, 0);
        }

        /// <summary>Start scrolling based on a fling gesture.</summary>
        /// <remarks>
        ///     Start scrolling based on a fling gesture. The distance traveled will
        ///     depend on the initial velocity of the fling.
        /// </remarks>
        /// <param name="startX">Starting point of the scroll (X)</param>
        /// <param name="startY">Starting point of the scroll (Y)</param>
        /// <param name="velocityX">
        ///     Initial velocity of the fling (X) measured in pixels per
        ///     second.
        /// </param>
        /// <param name="velocityY">
        ///     Initial velocity of the fling (Y) measured in pixels per
        ///     second
        /// </param>
        /// <param name="minX">
        ///     Minimum X value. The scroller will not scroll past this point
        ///     unless overX &gt; 0. If overfling is allowed, it will use minX as
        ///     a springback boundary.
        /// </param>
        /// <param name="maxX">
        ///     Maximum X value. The scroller will not scroll past this point
        ///     unless overX &gt; 0. If overfling is allowed, it will use maxX as
        ///     a springback boundary.
        /// </param>
        /// <param name="minY">
        ///     Minimum Y value. The scroller will not scroll past this point
        ///     unless overY &gt; 0. If overfling is allowed, it will use minY as
        ///     a springback boundary.
        /// </param>
        /// <param name="maxY">
        ///     Maximum Y value. The scroller will not scroll past this point
        ///     unless overY &gt; 0. If overfling is allowed, it will use maxY as
        ///     a springback boundary.
        /// </param>
        /// <param name="overX">
        ///     Overfling range. If &gt; 0, horizontal overfling in either
        ///     direction will be possible.
        /// </param>
        /// <param name="overY">
        ///     Overfling range. If &gt; 0, vertical overfling in either
        ///     direction will be possible.
        /// </param>
        public virtual void fling(Int32 startX,
                                  Int32 startY,
                                  Int32 velocityX,
                                  Int32 velocityY,
                                  Int32
                                      minX,
                                  Int32 maxX,
                                  Int32 minY,
                                  Int32 maxY,
                                  Int32 overX,
                                  Int32 overY)
        {
            // Continue a scroll or fling in progress
            if (mFlywheel && !isFinished())
            {
                var oldVelocityX = mScrollerX.mCurrVelocity;
                var oldVelocityY = mScrollerY.mCurrVelocity;
                if (Math.Sign(velocityX) == Math.Sign(oldVelocityX) && Math.Sign
                    (velocityY) == Math.Sign(oldVelocityY))
                {
                    velocityX += (Int32) oldVelocityX;
                    velocityY += (Int32) oldVelocityY;
                }
            }

            mMode = FLING_MODE;
            mScrollerX.fling(startX, velocityX, minX, maxX, overX);
            mScrollerY.fling(startY, velocityY, minY, maxY, overY);
        }

        /// <summary>Force the finished field to a particular value.</summary>
        /// <remarks>
        ///     Force the finished field to a particular value. Contrary to
        ///     <see cref="abortAnimation()">abortAnimation()</see>
        ///     , forcing the animation to finished
        ///     does NOT cause the scroller to move to the final x and y
        ///     position.
        /// </remarks>
        /// <param name="finished">The new finished value.</param>
        public void forceFinished(Boolean finished)
        {
            mScrollerX.mFinished = mScrollerY.mFinished = finished;
        }

        /// <summary>Returns the absolute value of the current velocity.</summary>
        /// <remarks>Returns the absolute value of the current velocity.</remarks>
        /// <returns>
        ///     The original velocity less the deceleration, norm of the X and Y velocity vector.
        /// </returns>
        public virtual Single getCurrVelocity()
        {
            var squaredNorm = mScrollerX.mCurrVelocity * mScrollerX.mCurrVelocity;
            squaredNorm += mScrollerY.mCurrVelocity * mScrollerY.mCurrVelocity;
            return FloatMath.Sqrt(squaredNorm);
        }

        /// <summary>Returns the current X offset in the scroll.</summary>
        /// <remarks>Returns the current X offset in the scroll.</remarks>
        /// <returns>The new X offset as an absolute distance from the origin.</returns>
        public Int32 getCurrX()
        {
            return mScrollerX.mCurrentPosition;
        }

        /// <summary>Returns the current Y offset in the scroll.</summary>
        /// <remarks>Returns the current Y offset in the scroll.</remarks>
        /// <returns>The new Y offset as an absolute distance from the origin.</returns>
        public Int32 getCurrY()
        {
            return mScrollerY.mCurrentPosition;
        }

        /// <summary>Returns how long the scroll event will take, in milliseconds.</summary>
        /// <remarks>Returns how long the scroll event will take, in milliseconds.</remarks>
        /// <returns>The duration of the scroll in milliseconds.</returns>
        /// <hide>Pending removal once nothing depends on it</hide>
        [ObsoleteAttribute(
            @"OverScrollers don't necessarily have a fixed duration. This function will lie to the best of its ability."
        )]
        public Int32 getDuration()
        {
            return Math.Max(mScrollerX.mDuration, mScrollerY.mDuration);
        }

        /// <summary>Returns where the scroll will end.</summary>
        /// <remarks>Returns where the scroll will end. Valid only for "fling" scrolls.</remarks>
        /// <returns>The final X offset as an absolute distance from the origin.</returns>
        public Int32 getFinalX()
        {
            return mScrollerX.mFinal;
        }

        /// <summary>Returns where the scroll will end.</summary>
        /// <remarks>Returns where the scroll will end. Valid only for "fling" scrolls.</remarks>
        /// <returns>The final Y offset as an absolute distance from the origin.</returns>
        public Int32 getFinalY()
        {
            return mScrollerY.mFinal;
        }

        /// <summary>Returns the start X offset in the scroll.</summary>
        /// <remarks>Returns the start X offset in the scroll.</remarks>
        /// <returns>The start X offset as an absolute distance from the origin.</returns>
        public Int32 getStartX()
        {
            return mScrollerX.mStart;
        }

        /// <summary>Returns the start Y offset in the scroll.</summary>
        /// <remarks>Returns the start Y offset in the scroll.</remarks>
        /// <returns>The start Y offset as an absolute distance from the origin.</returns>
        public Int32 getStartY()
        {
            return mScrollerY.mStart;
        }

        /// <summary>Returns whether the scroller has finished scrolling.</summary>
        /// <remarks>Returns whether the scroller has finished scrolling.</remarks>
        /// <returns>True if the scroller has finished scrolling, false otherwise.</returns>
        public Boolean isFinished()
        {
            return mScrollerX.mFinished && mScrollerY.mFinished;
        }

        /// <summary>
        ///     Returns whether the current Scroller is currently returning to a valid position.
        /// </summary>
        /// <remarks>
        ///     Returns whether the current Scroller is currently returning to a valid position.
        ///     Valid bounds were provided by the
        ///     <see cref="fling(int, int, int, int, int, int, int, int, int, int)">
        ///         fling(int, int, int, int, int, int, int, int, int, int)
        ///     </see>
        ///     method.
        ///     One should check this value before calling
        ///     <see cref="startScroll(int, int, int, int)">startScroll(int, int, int, int)</see>
        ///     as the interpolation currently in progress
        ///     to restore a valid position will then be stopped. The caller has to take into account
        ///     the fact that the started scroll will start from an overscrolled position.
        /// </remarks>
        /// <returns>
        ///     true when the current position is overscrolled and in the process of
        ///     interpolating back to a valid value.
        /// </returns>
        public virtual Boolean isOverScrolled()
        {
            return !mScrollerX.mFinished && mScrollerX.mState != SplineOverScroller.SPLINE ||
                   !mScrollerY.mFinished && mScrollerY.mState != SplineOverScroller.SPLINE;
        }

        /// <hide></hide>
        public virtual Boolean isScrollingInDirection(Single xvel,
                                                      Single yvel)
        {
            var dx = mScrollerX.mFinal - mScrollerX.mStart;
            var dy = mScrollerY.mFinal - mScrollerY.mStart;
            return !isFinished() && Math.Sign(xvel) == Math.Sign(dx) && Math.Sign
                (yvel) == Math.Sign(dy);
        }

        /// <summary>Notify the scroller that we've reached a horizontal boundary.</summary>
        /// <remarks>
        ///     Notify the scroller that we've reached a horizontal boundary.
        ///     Normally the information to handle this will already be known
        ///     when the animation is started, such as in a call to one of the
        ///     fling functions. However there are cases where this cannot be known
        ///     in advance. This function will transition the current motion and
        ///     animate from startX to finalX as appropriate.
        /// </remarks>
        /// <param name="startX">Starting/current X position</param>
        /// <param name="finalX">Desired final X position</param>
        /// <param name="overX">
        ///     Magnitude of overscroll allowed. This should be the maximum
        ///     desired distance from finalX. Absolute value - must be positive.
        /// </param>
        public virtual void notifyHorizontalEdgeReached(Int32 startX,
                                                        Int32 finalX,
                                                        Int32 overX
        )
        {
            mScrollerX.notifyEdgeReached(startX, finalX, overX);
        }

        /// <summary>Notify the scroller that we've reached a vertical boundary.</summary>
        /// <remarks>
        ///     Notify the scroller that we've reached a vertical boundary.
        ///     Normally the information to handle this will already be known
        ///     when the animation is started, such as in a call to one of the
        ///     fling functions. However there are cases where this cannot be known
        ///     in advance. This function will animate a parabolic motion from
        ///     startY to finalY.
        /// </remarks>
        /// <param name="startY">Starting/current Y position</param>
        /// <param name="finalY">Desired final Y position</param>
        /// <param name="overY">
        ///     Magnitude of overscroll allowed. This should be the maximum
        ///     desired distance from finalY. Absolute value - must be positive.
        /// </param>
        public virtual void notifyVerticalEdgeReached(Int32 startY,
                                                      Int32 finalY,
                                                      Int32 overY)
        {
            mScrollerY.notifyEdgeReached(startY, finalY, overY);
        }

        /// <summary>Sets the final position (X) for this scroller.</summary>
        /// <remarks>Sets the final position (X) for this scroller.</remarks>
        /// <param name="newX">The new X offset as an absolute distance from the origin.</param>
        /// <seealso cref="extendDuration(int)">extendDuration(int)</seealso>
        /// <seealso cref="setFinalY(int)">setFinalY(int)</seealso>
        /// <hide>Pending removal once nothing depends on it</hide>
        [ObsoleteAttribute(
            @"OverScroller's final position may change during an animation. Instead of setting a new final position and extending the duration of an existing scroll, use startScroll to begin a new animation."
        )]
        public virtual void setFinalX(Int32 newX)
        {
            mScrollerX.setFinalPosition(newX);
        }

        /// <summary>Sets the final position (Y) for this scroller.</summary>
        /// <remarks>Sets the final position (Y) for this scroller.</remarks>
        /// <param name="newY">The new Y offset as an absolute distance from the origin.</param>
        /// <seealso cref="extendDuration(int)">extendDuration(int)</seealso>
        /// <seealso cref="setFinalX(int)">setFinalX(int)</seealso>
        /// <hide>Pending removal once nothing depends on it</hide>
        [ObsoleteAttribute(
            @"OverScroller's final position may change during an animation. Instead of setting a new final position and extending the duration of an existing scroll, use startScroll to begin a new animation."
        )]
        public virtual void setFinalY(Int32 newY)
        {
            mScrollerY.setFinalPosition(newY);
        }

        /// <summary>The amount of friction applied to flings.</summary>
        /// <remarks>
        ///     The amount of friction applied to flings. The default value
        ///     is
        ///     <see cref="ViewConfiguration.getScrollFriction()">
        ///         ViewConfiguration.getScrollFriction()
        ///     </see>
        ///     .
        /// </remarks>
        /// <param name="friction">
        ///     A scalar dimension-less value representing the coefficient of
        ///     friction.
        /// </param>
        public void setFriction(Single friction)
        {
            mScrollerX.setFriction(friction);
            mScrollerY.setFriction(friction);
        }

        /// <summary>Call this when you want to 'spring back' into a valid coordinate range.</summary>
        /// <remarks>Call this when you want to 'spring back' into a valid coordinate range.</remarks>
        /// <param name="startX">Starting X coordinate</param>
        /// <param name="startY">Starting Y coordinate</param>
        /// <param name="minX">Minimum valid X value</param>
        /// <param name="maxX">Maximum valid X value</param>
        /// <param name="minY">Minimum valid Y value</param>
        /// <param name="maxY">Minimum valid Y value</param>
        /// <returns>
        ///     true if a springback was initiated, false if startX and startY were
        ///     already within the valid range.
        /// </returns>
        public virtual Boolean springBack(Int32 startX,
                                          Int32 startY,
                                          Int32 minX,
                                          Int32 maxX,
                                          Int32 minY,
                                          Int32 maxY)
        {
            mMode = FLING_MODE;
            // Make sure both methods are called.
            var spingbackX = mScrollerX.springback(startX, minX, maxX);
            var spingbackY = mScrollerY.springback(startY, minY, maxY);
            return spingbackX || spingbackY;
        }

        /// <summary>
        ///     Start scrolling by providing a starting point and the distance to travel.
        /// </summary>
        /// <remarks>
        ///     Start scrolling by providing a starting point and the distance to travel.
        ///     The scroll will use the default value of 250 milliseconds for the
        ///     duration.
        /// </remarks>
        /// <param name="startX">
        ///     Starting horizontal scroll offset in pixels. Positive
        ///     numbers will scroll the content to the left.
        /// </param>
        /// <param name="startY">
        ///     Starting vertical scroll offset in pixels. Positive numbers
        ///     will scroll the content up.
        /// </param>
        /// <param name="dx">
        ///     Horizontal distance to travel. Positive numbers will scroll the
        ///     content to the left.
        /// </param>
        /// <param name="dy">
        ///     Vertical distance to travel. Positive numbers will scroll the
        ///     content up.
        /// </param>
        public virtual void StartScroll(Int32 startX,
                                        Int32 startY,
                                        Int32 dx,
                                        Int32 dy)
        {
            startScroll(startX, startY, dx, dy, DEFAULT_DURATION);
        }

        /// <summary>
        ///     Start scrolling by providing a starting point and the distance to travel.
        /// </summary>
        /// <remarks>
        ///     Start scrolling by providing a starting point and the distance to travel.
        /// </remarks>
        /// <param name="startX">
        ///     Starting horizontal scroll offset in pixels. Positive
        ///     numbers will scroll the content to the left.
        /// </param>
        /// <param name="startY">
        ///     Starting vertical scroll offset in pixels. Positive numbers
        ///     will scroll the content up.
        /// </param>
        /// <param name="dx">
        ///     Horizontal distance to travel. Positive numbers will scroll the
        ///     content to the left.
        /// </param>
        /// <param name="dy">
        ///     Vertical distance to travel. Positive numbers will scroll the
        ///     content up.
        /// </param>
        /// <param name="duration">Duration of the scroll in milliseconds.</param>
        public virtual void startScroll(Int32 startX,
                                        Int32 startY,
                                        Int32 dx,
                                        Int32 dy,
                                        Int32 duration
        )
        {
            mMode = SCROLL_MODE;
            mScrollerX.startScroll(startX, dx, duration);
            mScrollerY.startScroll(startY, dy, duration);
        }

       

        /// <summary>Returns the time elapsed since the beginning of the scrolling.</summary>
        /// <remarks>Returns the time elapsed since the beginning of the scrolling.</remarks>
        /// <returns>The elapsed time in milliseconds.</returns>
        /// <hide></hide>
        public virtual Int32 timePassed()
        {
            var time = AnimationUtils.CurrentAnimationTimeMillis();
            var startTime = Math.Min(mScrollerX.mStartTime, mScrollerY.mStartTime);
            return (Int32) (time - startTime);
        }

        internal static Single viscousFluid(Single x)
        {
            x *= sViscousFluidScale;
            if (x < 1.0f)
                x -= 1.0f - (Single) Math.Exp(-x);
            else
            {
                var start = 0.36787944117f;
                // 1/e == exp(-1)
                x = 1.0f - (Single) Math.Exp(1.0f - x);
                x = start + x * (1.0f - start);
            }

            x *= sViscousFluidNormalize;
            return x;
        }

        internal const Int32 DEFAULT_DURATION = 250;

        internal const Int32 SCROLL_MODE = 0;

        internal const Int32 FLING_MODE = 1;

        private static readonly Single sViscousFluidScale;

        private static readonly Single sViscousFluidNormalize;

        private readonly Boolean mFlywheel;

        private readonly Interpolator? mInterpolator;

        private readonly SplineOverScroller mScrollerX;

        private readonly SplineOverScroller mScrollerY;
        private Int32 mMode;

        internal class SplineOverScroller
        {
            static SplineOverScroller()
            {
                // Initial position
                // Current position
                // Final position
                // Initial velocity
                // Current velocity
                // Constant current deceleration
                // Animation starting time, in system milliseconds
                // Animation duration, in milliseconds
                // Duration to complete spline component of animation
                // Distance to travel along spline animation
                // Whether the animation is currently in progress
                // The allowed overshot distance before boundary is reached.
                // Fling friction
                // Current state of the animation.
                // Constant gravity value, used in the deceleration phase.
                // A device specific coefficient adjusted to physical values.
                // Tension lines cross at (INFLEXION, 1)
                var x_min = 0.0f;
                var y_min = 0.0f;
                {
                    for (var i = 0; i < NB_SAMPLES; i++)
                    {
                        var alpha = (Single) i / NB_SAMPLES;
                        var x_max = 1.0f;
                        Single x;
                        Single tx;
                        Single coef;
                        while (true)
                        {
                            x = x_min + (x_max - x_min) / 2.0f;
                            coef = 3.0f * x * (1.0f - x);
                            tx = coef * ((1.0f - x) * P1 + x * P2) + x * x * x;
                            if (Math.Abs(tx - alpha) < 1E-5)
                                break;
                            if (tx > alpha)
                                x_max = x;
                            else
                                x_min = x;
                        }

                        SPLINE_POSITION[i] = coef * ((1.0f - x) * START_TENSION + x) + x * x * x;
                        var y_max = 1.0f;
                        Single y;
                        Single dy;
                        while (true)
                        {
                            y = y_min + (y_max - y_min) / 2.0f;
                            coef = 3.0f * y * (1.0f - y);
                            dy = coef * ((1.0f - y) * START_TENSION + y) + y * y * y;
                            if (Math.Abs(dy - alpha) < 1E-5)
                                break;
                            if (dy > alpha)
                                y_max = y;
                            else
                                y_min = y;
                        }

                        SPLINE_TIME[i] = coef * ((1.0f - y) * P1 + y * P2) + y * y * y;
                    }
                }
                SPLINE_POSITION[NB_SAMPLES] = SPLINE_TIME[NB_SAMPLES] = 1.0f;
            }

            internal SplineOverScroller()
            {
                mFinished = true;
                mCurrVelocity = 0;
                mSplineDuration = 0;
            }

            public static Int32 Round(Single f)
            {
                return (Int32) Math.Round(f);
            }

            internal virtual Boolean continueWhenFinished()
            {
                switch (mState)
                {
                    case SPLINE:
                    {
                        // Duration from start to null velocity
                        if (mDuration < mSplineDuration)
                        {
                            // If the animation was clamped, we reached the edge
                            mStart = mFinal;
                            // TODO Better compute speed when edge was reached
                            mVelocity = (Int32) mCurrVelocity;
                            mDeceleration = getDeceleration(mVelocity);
                            mStartTime += mDuration;
                            onEdgeReached();
                        }
                        else
                            // Normal stop, no need to continue
                            return false;

                        break;
                    }

                    case BALLISTIC:
                    {
                        mStartTime += mDuration;
                        startSpringback(mFinal, mStart, 0);
                        break;
                    }

                    case CUBIC:
                    {
                        return false;
                    }
                }

                update();
                return true;
            }

            internal virtual void extendDuration(Int32 extend)
            {
                var time = AnimationUtils.CurrentAnimationTimeMillis();
                var elapsedTime = (Int32) (time - mStartTime);
                mDuration = elapsedTime + extend;
                mFinished = false;
            }

            internal virtual void finish()
            {
                mCurrentPosition = mFinal;
                // Not reset since WebView relies on this value for fast fling.
                // TODO: restore when WebView uses the fast fling implemented in this class.
                // mCurrVelocity = 0.0f;
                mFinished = true;
            }

            internal virtual void fling(Int32 start,
                                        Int32 velocity,
                                        Int32 min,
                                        Int32 max,
                                        Int32 over)
            {
                mOver = over;
                mFinished = false;
                mCurrVelocity = mVelocity = velocity;
                mDuration = mSplineDuration = 0;
                mStartTime = AnimationUtils.CurrentAnimationTimeMillis();
                mCurrentPosition = mStart = start;
                if (start > max || start < min)
                {
                    startAfterEdge(start, min, max, velocity);
                    return;
                }

                mState = SPLINE;
                var totalDistance = 0.0;
                if (velocity != 0)
                {
                    mDuration = mSplineDuration = getSplineFlingDuration(velocity);
                    totalDistance = getSplineFlingDistance(velocity);
                }

                mSplineDistance = (Int32) (totalDistance * Math.Sign(velocity));
                mFinal = start + mSplineDistance;
                // Clamp to a valid final position
                if (mFinal < min)
                {
                    adjustDuration(mStart, mFinal, min);
                    mFinal = min;
                }

                if (mFinal > max)
                {
                    adjustDuration(mStart, mFinal, max);
                    mFinal = max;
                }

                if (mDuration > 0)
                    BadLog.WriteLine(":+::+::+::+::+:SPLINE:+::+::+::+::+::+:\r\n" +
                                     "dist: " + mSplineDistance + " time: " +
                                     mDuration + " V0: " + velocity +
                                     "start pos: " + start +
                                     " final: " + mFinal);
            }

            internal static void initFromContext(Context context)
            {
                var ppi = context.Resources.DisplayMetrics.Density * 160.0f;
                PHYSICAL_COEF = SensorManager.GravityEarth * 39.37f * ppi * 0.84f;
            }

            internal virtual void notifyEdgeReached(Int32 start,
                                                    Int32 end,
                                                    Int32 over)
            {
                // mState is used to detect successive notifications 
                if (mState == SPLINE)
                {
                    mOver = over;
                    mStartTime = AnimationUtils.CurrentAnimationTimeMillis();
                    // We were in fling/scroll mode before: current velocity is such that distance to
                    // edge is increasing. This ensures that startAfterEdge will not start a new fling.
                    startAfterEdge(start, end, end, (Int32) mCurrVelocity);
                }
            }

            internal virtual void setFinalPosition(Int32 position)
            {
                mFinal = position;
                mFinished = false;
            }

            // g (m/s^2)
            // inch/meter
            // look and feel tuning
            internal virtual void setFriction(Single friction)
            {
                mFlingFriction = friction;
            }

            internal virtual Boolean springback(Int32 start,
                                                Int32 min,
                                                Int32 max)
            {
                mFinished = true;
                mStart = mFinal = start;
                mVelocity = 0;
                mStartTime = AnimationUtils.CurrentAnimationTimeMillis();
                mDuration = 0;
                if (start < min)
                    startSpringback(start, min, 0);
                else
                {
                    if (start > max)
                        startSpringback(start, max, 0);
                }

                return !mFinished;
            }

            internal virtual void startScroll(Int32 start,
                                              Int32 distance,
                                              Int32 duration)
            {
                mFinished = false;
                mStart = start;
                mFinal = start + distance;
                mStartTime = AnimationUtils.CurrentAnimationTimeMillis();
                mDuration = duration;
                // Unused
                mDeceleration = 0.0f;
                mVelocity = 0;
            }

            internal virtual Boolean update()
            {
                var time = AnimationUtils.CurrentAnimationTimeMillis();
                var currentTime = time - mStartTime;
                if (currentTime > mDuration)
                    return false;
                var distance = 0.0;
                switch (mState)
                {
                    case SPLINE:
                    {
                        var t = (Single) currentTime / mSplineDuration;
                        var index = (Int32) (NB_SAMPLES * t);
                        var distanceCoef = 1.0f;
                        var velocityCoef = 0.0f;
                        if (index < NB_SAMPLES)
                        {
                            var t_inf = (Single) index / NB_SAMPLES;
                            var t_sup = (Single) (index + 1) / NB_SAMPLES;
                            var d_inf = SPLINE_POSITION[index];
                            var d_sup = SPLINE_POSITION[index + 1];
                            velocityCoef = (d_sup - d_inf) / (t_sup - t_inf);
                            distanceCoef = d_inf + (t - t_inf) * velocityCoef;
                        }

                        distance = distanceCoef * mSplineDistance;
                        mCurrVelocity = velocityCoef * mSplineDistance / mSplineDuration * 1000.0f;
                        break;
                    }

                    case BALLISTIC:
                    {
                        var t = currentTime / 1000.0f;
                        mCurrVelocity = mVelocity + mDeceleration * t;
                        distance = mVelocity * t + mDeceleration * t * t / 2.0f;
                        break;
                    }

                    case CUBIC:
                    {
                        var t = (Single) currentTime / mDuration;
                        var t2 = t * t;
                        Single sign = Math.Sign(mVelocity);
                        distance = sign * mOver * (3.0f * t2 - 2.0f * t * t2);
                        mCurrVelocity = sign * mOver * 6.0f * (-t + t2);
                        break;
                    }
                }

                mCurrentPosition = mStart + (Int32) Math.Round(distance);
                return true;
            }

            internal virtual void updateScroll(Single q)
            {
                mCurrentPosition = mStart + Round(q * (mFinal - mStart));
            }

            private void adjustDuration(Int32 start,
                                        Int32 oldFinal,
                                        Int32 newFinal)
            {
                var oldDistance = oldFinal - start;
                var newDistance = newFinal - start;
                var x = Math.Abs((Single) newDistance / oldDistance);
                var index = (Int32) (NB_SAMPLES * x);
                if (index < NB_SAMPLES)
                {
                    var x_inf = (Single) index / NB_SAMPLES;
                    var x_sup = (Single) (index + 1) / NB_SAMPLES;
                    var t_inf = SPLINE_TIME[index];
                    var t_sup = SPLINE_TIME[index + 1];
                    var timeCoef = t_inf + (x - x_inf) / (x_sup - x_inf) * (t_sup - t_inf);
                    mDuration *= (Int32) timeCoef;
                }
            }

            private void fitOnBounceCurve(Int32 start,
                                          Int32 end,
                                          Int32 velocity)
            {
                // Simulate a bounce that started from edge
                var durationToApex = -velocity / mDeceleration;
                var distanceToApex = velocity * velocity / 2.0f / Math.Abs(mDeceleration
                );
                Single distanceToEdge = Math.Abs(end - start);
                var totalDuration = (Single) Math.Sqrt(2.0 * (distanceToApex + distanceToEdge
                    ) / Math.Abs(mDeceleration));
                mStartTime -= (Int32) (1000.0f * (totalDuration - durationToApex));
                mStart = end;
                mVelocity = (Int32) (-mDeceleration * totalDuration);
            }

            private static Single getDeceleration(Int32 velocity)
            {
                return velocity > 0 ? -GRAVITY : GRAVITY;
            }

            private Double getSplineDeceleration(Int32 velocity)
            {
                return Math.Log(INFLEXION * Math.Abs(velocity) / (mFlingFriction *
                                                                  PHYSICAL_COEF));
            }

            private Double getSplineFlingDistance(Int32 velocity)
            {
                var l = getSplineDeceleration(velocity);
                var decelMinusOne = DECELERATION_RATE - 1.0;
                return mFlingFriction * PHYSICAL_COEF * Math.Exp(DECELERATION_RATE / decelMinusOne
                                                                 * l);
            }

            private Int32 getSplineFlingDuration(Int32 velocity)
            {
                var l = getSplineDeceleration(velocity);
                var decelMinusOne = DECELERATION_RATE - 1.0;
                return (Int32) (1000.0 * Math.Exp(l / decelMinusOne));
            }

            private void onEdgeReached()
            {
                // mStart, mVelocity and mStartTime were adjusted to their values when edge was reached.
                var distance = mVelocity * mVelocity / (2.0f * Math.Abs(mDeceleration));
                Single sign = Math.Sign(mVelocity);
                if (distance > mOver)
                {
                    // Default deceleration is not sufficient to slow us down before boundary
                    mDeceleration = -sign * mVelocity * mVelocity / (2.0f * mOver);
                    distance = mOver;
                }

                mOver = (Int32) distance;
                mState = BALLISTIC;
                mFinal = mStart + (Int32) (mVelocity > 0 ? distance : -distance);
                mDuration = -(Int32) (1000.0f * mVelocity / mDeceleration);
            }

            private void startAfterEdge(Int32 start,
                                        Int32 min,
                                        Int32 max,
                                        Int32 velocity)
            {
                if (start > min && start < max)
                {
                    //Log.e("OverScroller", "startAfterEdge called from a valid position");
                    mFinished = true;
                    return;
                }

                var positive = start > max;
                var edge = positive ? max : min;
                var overDistance = start - edge;
                var keepIncreasing = overDistance * velocity >= 0;
                if (keepIncreasing)
                    // Will result in a bounce or a to_boundary depending on velocity.
                    startBounceAfterEdge(start, edge, velocity);
                else
                {
                    var totalDistance = getSplineFlingDistance(velocity);
                    if (totalDistance > Math.Abs(overDistance))
                        fling(start, velocity, positive ? min : start, positive ? start : max, mOver);
                    else
                        startSpringback(start, edge, velocity);
                }
            }

            private void startBounceAfterEdge(Int32 start,
                                              Int32 end,
                                              Int32 velocity)
            {
                mDeceleration = getDeceleration(velocity == 0 ? start - end : velocity);
                fitOnBounceCurve(start, end, velocity);
                onEdgeReached();
            }

            private void startSpringback(Int32 start,
                                         Int32 end,
                                         Int32 velocity)
            {
                // mStartTime has been set
                mFinished = false;
                mState = CUBIC;
                mStart = start;
                mFinal = end;
                var delta = start - end;
                mDeceleration = getDeceleration(delta);
                // TODO take velocity into account
                mVelocity = -delta;
                // only sign is used
                mOver = Math.Abs(delta);
                mDuration = (Int32) (1000.0 * Math.Sqrt(-2.0 * delta / mDeceleration));
            }

            internal const Single GRAVITY = 2000.0f;

            internal const Single INFLEXION = 0.35f;

            internal const Single START_TENSION = 0.5f;

            internal const Single END_TENSION = 1.0f;

            internal const Single P1 = START_TENSION * INFLEXION;

            internal const Single P2 = 1.0f - END_TENSION * (1.0f - INFLEXION);

            internal const Int32 NB_SAMPLES = 100;

            internal const Int32 SPLINE = 0;

            internal const Int32 CUBIC = 1;

            internal const Int32 BALLISTIC = 2;

            internal static Single PHYSICAL_COEF;

            internal static Single DECELERATION_RATE = (Single) (Math.Log(0.78) / Math.Log
                (0.9));

            internal static readonly Single[] SPLINE_POSITION = new Single[NB_SAMPLES + 1];

            internal static readonly Single[] SPLINE_TIME = new Single[NB_SAMPLES + 1];

            internal Int32 mCurrentPosition;

            private Single _mCurrVelocity;

            internal Single mCurrVelocity
            {
                get => _mCurrVelocity;
                set
                {
                    if (Double.IsNaN(value))
                    {
                        _mCurrVelocity = 0;
                    }
                    else
                        _mCurrVelocity = value;
                }
            }

            internal Single mDeceleration;

            internal Int32 mDuration;

            internal Int32 mFinal;

            internal Boolean mFinished;

            internal Single mFlingFriction = ViewConfiguration.ScrollFriction;

            internal Int32 mOver;

            internal Int32 mSplineDistance;

            private Int32 _mSplineDuration;
            internal Int32 mSplineDuration
            {
                get => _mSplineDuration;
                set
                {
                    if (Double.IsNaN(value))
                    {}
                    _mSplineDuration = value;
                }
            }
            internal Int32 mStart;

            internal Int64 mStartTime;

            internal Int32 mState = SPLINE;

            internal Int32 mVelocity;
        }
    }
}
