using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Util;
using Android.Views.Animations;

//using visky = Android.Widget.Scroller.ViscousFluidInterpolator;

namespace XamarinAndroidNativeTest
{
    public class OverScrollerToo : IOverScroller
    {
        public OverScrollerToo(Context context)
            : this(context, null)
        {
        }

        /**
     * Creates an OverScroller with flywheel enabled.
     * @param context The context of this application.
     * @param interpolator The scroll interpolator. If null, a default (viscous) interpolator will
     * be used.
     */
        public OverScrollerToo(Context context,
                               IInterpolator? interpolator)
            : this(context, interpolator, true)
        {
        }

        public OverScrollerToo(Context context,
                               IInterpolator? interpolator,
                               Boolean flywheel)
        {
            if (interpolator == null)
                mInterpolator = new ViscousFluidInterpolator();
            else
                mInterpolator = interpolator;
            mFlywheel = flywheel;
            _mScrollerTooX = new SplineOverScrollerToo(context, "X");
            _mScrollerTooY = new SplineOverScrollerToo(context, "Y");
        }

        public Single CurrentYVelocity => _mScrollerTooY.mCurrVelocity;

        public Single CurrVelocity => FloatMath.Hypot(_mScrollerTooX.mCurrVelocity, _mScrollerTooY.mCurrVelocity);

        public Single CurrY => _mScrollerTooY.mCurrentPosition;

        public Int32 FinalY => _mScrollerTooY.mFinal;

        //private double getSplineFlingDistance(int velocity) {
        //    var l = getSplineDeceleration(velocity);
        //    var decelMinusOne = DECELERATION_RATE - 1.0;
        //    return mFlingFriction * mPhysicalCoeff * Math.Exp(DECELERATION_RATE / decelMinusOne * l);
        //}

        //private double getSplineDeceleration(int velocity) {
        //    return Math.Log(INFLEXION * Math.Abs(velocity) / (mFlingFriction * mPhysicalCoeff));
        //}


        //private void fitOnBounceCurve(Int32 start,
        //                              Int32 end,
        //                              Int32 velocity)
        //{
        //    // Simulate a bounce that started from edge
        //    var durationToApex = -velocity / mDeceleration;
        //    // The float cast below is necessary to avoid integer overflow.
        //    var velocitySquared = (Single) velocity * velocity;
        //    var distanceToApex = velocitySquared / 2.0f / Math.Abs(mDeceleration);
        //    var distanceToEdge = Math.Abs(end - start);
        //    var totalDuration = (Single) Math.Sqrt(
        //        2.0 * (distanceToApex + distanceToEdge) / Math.Abs(mDeceleration));
        //    mStartTime -= (Int32) (1000.0f * (totalDuration - durationToApex));
        //    mCurrentPosition = mStart = end;
        //    mVelocity = (Int32) (-mDeceleration * totalDuration);
        //}

        //private static Single getDeceleration(Int32 velocity)
        //{
        //    return velocity > 0 ? -GRAVITY : GRAVITY;
        //}


        //private void onEdgeReached()
        //{
        //    // mStart, mVelocity and mStartTime were adjusted to their values when edge was reached.
        //    // The float cast below is necessary to avoid integer overflow.
        //    var velocitySquared = (Single) mVelocity * mVelocity;
        //    var distance = velocitySquared / (2.0f * Math.Abs(mDeceleration));
        //    var sign = Math.Sign(mVelocity);
        //    if (distance > mOver)
        //    {
        //        // Default deceleration is not sufficient to slow us down before boundary
        //        mDeceleration = -sign * velocitySquared / (2.0f * mOver);
        //        distance = mOver;
        //    }

        //    mOver = (Int32) distance;
        //    mState = BALLISTIC;
        //    mFinal = mStart + (Int32) (mVelocity > 0 ? distance : -distance);
        //    mDuration = -(Int32) (1000.0f * mVelocity / mDeceleration);
        //}

        //private void startBounceAfterEdge(Int32 start,
        //                                  Int32 end,
        //                                  Int32 velocity)
        //{
        //    mDeceleration = getDeceleration(velocity == 0 ? start - end : velocity);
        //    fitOnBounceCurve(start, end, velocity);
        //    onEdgeReached();
        //}

        //private void startAfterEdge(int start, int min, int max, int velocity) {
        //    if (start > min && start < max) {
        //        //Log.e("OverScroller", "startAfterEdge called from a valid position");
        //        mFinished = true;
        //        return;
        //    }
        //    var positive = start > max;
        //    var edge = positive ? max : min;
        //    var overDistance = start - edge;
        //    var keepIncreasing = overDistance * velocity >= 0;
        //    if (keepIncreasing) {
        //        // Will result in a bounce or a to_boundary depending on velocity.
        //        startBounceAfterEdge(start, edge, velocity);
        //    } else {
        //        var totalDistance = getSplineFlingDistance(velocity);
        //        if (totalDistance > Math.Abs(overDistance)) {
        //            fling(start, velocity, positive ? min : start, positive ? start : max, mOver);
        //        } else {
        //            startSpringback(start, edge, velocity);
        //        }
        //    }
        //}

        //private void startSpringback(Int32 start,
        //                             Int32 end,
        //                             Int32 velocity)
        //{
        //    // mStartTime has been set
        //    mFinished = false;
        //    mState = CUBIC;
        //    mCurrentPosition = mStart = start;
        //    mFinal = end;
        //    var delta = start - end;
        //    mDeceleration = getDeceleration(delta);
        //    // TODO take velocity into account
        //    mVelocity = -delta; // only sign is used
        //    mOver = Math.Abs(delta);
        //    mDuration = (Int32) (1000.0 * Math.Sqrt(-2.0 * delta / mDeceleration));
        //}

        public Int32 StartY => _mScrollerTooY.mStart;

        public void abortAnimation()
        {
            _mScrollerTooX.finish();
            _mScrollerTooY.finish();
        }

        public Boolean computeScrollOffset()
        {
            if (isFinished())
                return false;

            switch (mMode)
            {
                case SCROLL_MODE:
                    var time = AnimationUtils.CurrentAnimationTimeMillis();
                    // Any scroller can be used for time, since they were started
                    // together in scroll mode. We use X here.
                    var elapsedTime = time - _mScrollerTooX.mStartTime;
                    var duration = _mScrollerTooX.mDuration;
                    if (elapsedTime < duration)
                    {
                        var q = mInterpolator.GetInterpolation(elapsedTime / (Single) duration);
                        _mScrollerTooX.updateScroll(q);
                        _mScrollerTooY.updateScroll(q);
                    }
                    else
                        abortAnimation();

                    break;
                case FLING_MODE:
                    if (!_mScrollerTooX.mFinished)
                        if (!_mScrollerTooX.update())
                            if (!_mScrollerTooX.continueWhenFinished())
                                _mScrollerTooX.finish();
                    if (!_mScrollerTooY.mFinished)
                        if (!_mScrollerTooY.update())
                            if (!_mScrollerTooY.continueWhenFinished())
                                _mScrollerTooY.finish();
                    break;
            }

            return true;
        }

        public void fling(Int32 startX,
                          Int32 startY,
                          Int32 velocityX,
                          Int32 velocityY,
                          Int32 minX,
                          Int32 maxX,
                          Int32 minY,
                          Int32 maxY,
                          Int32 overX,
                          Int32 overY)
        {
            // Continue a scroll or fling in progress
            if (mFlywheel && !isFinished())
            {
                var oldVelocityX = _mScrollerTooX.mCurrVelocity;
                var oldVelocityY = _mScrollerTooY.mCurrVelocity;
                if (Math.Sign(velocityX) == Math.Sign(oldVelocityX) &&
                    Math.Sign(velocityY) == Math.Sign(oldVelocityY))
                {
                    velocityX = Convert.ToInt32(velocityX + oldVelocityX);
                    velocityY = Convert.ToInt32(velocityY + oldVelocityY);
                }
            }

            mMode = FLING_MODE;
            _mScrollerTooX.fling(startX, velocityX, minX, maxX, overX);
            _mScrollerTooY.fling(startY, velocityY, minY, maxY, overY);
        }

        public Boolean isFinished()
        {
            return _mScrollerTooX.mFinished && _mScrollerTooY.mFinished;
        }

        public Boolean springBack(Int32 startX,
                                  Int32 startY,
                                  Int32 minX,
                                  Int32 maxX,
                                  Int32 minY,
                                  Int32 maxY)
        {
            mMode = FLING_MODE;
            // Make sure both methods are called.
            var spingbackX = _mScrollerTooX.springback(startX, minX, maxX);
            var spingbackY = _mScrollerTooY.springback(startY, minY, maxY);
            return spingbackX || spingbackY;
        }

        public void startScroll(Int32 startX,
                                Int32 startY,
                                Int32 dx,
                                Int32 dy,
                                Int32 duration)
        {
            mMode = SCROLL_MODE;
            _mScrollerTooX.startScroll(startX, dx, duration);
            _mScrollerTooY.startScroll(startY, dy, duration);
        }

        //public void fling(Int32 startX,
        //                  Int32 startY,
        //                  Int32 velocityX,
        //                  Int32 velocityY,
        //                  Int32 minX,
        //                  Int32 maxX,
        //                  Int32 minY,
        //                  Int32 maxY)
        //{
        //    fling(startX, startY, velocityX, velocityY, minX, maxX, minY, maxY, 0, 0);
        //}

        public void StartScroll(Int32 startX,
                                Int32 startY,
                                Int32 dx,
                                Int32 dy)
        {
            startScroll(startX, startY, dx, dy, DEFAULT_DURATION);
        }

        private const Int32 DEFAULT_DURATION = 250;
        private const Int32 SCROLL_MODE = 0;
        private const Int32 FLING_MODE = 1;

        //private const Int32 SPLINE = 0;
        //private const Int32 CUBIC = 1;
        //private const Int32 BALLISTIC = 2;
        //private const Single GRAVITY = 2000.0f;

        //private static Single DECELERATION_RATE = (Single) (Math.Log(0.78) / Math.Log(0.9));
        //private static readonly Single INFLEXION = 0.35f; // Tension lines cross at (INFLEXION, 1)
        //private static readonly Single START_TENSION = 0.5f;
        //private static readonly Single END_TENSION = 1.0f;
        //private static readonly Single P1 = START_TENSION * INFLEXION;

        //private static readonly Single P2 = 1.0f - END_TENSION * (1.0f - INFLEXION);

        //private readonly SplineOverScroller mScrollerX;
        private readonly SplineOverScrollerToo _mScrollerTooX;

        private readonly SplineOverScrollerToo _mScrollerTooY;

        private readonly Boolean mFlywheel;


        //void fling(int start, int velocity, int min, int max, int over) {
        //    mOver = over;
        //    mFinished = false;
        //    mCurrVelocity = mVelocity = velocity;
        //    mDuration = mSplineDuration = 0;
        //    mStartTime = AnimationUtils.CurrentAnimationTimeMillis();
        //    mCurrentPosition = mStart = start;
        //    if (start > max || start < min) {
        //        startAfterEdge(start, min, max, velocity);
        //        return;
        //    }
        //    mState = SPLINE;
        //    double totalDistance = 0.0;
        //    if (velocity != 0) {
        //        mDuration = mSplineDuration = getSplineFlingDuration(velocity);
        //        totalDistance = getSplineFlingDistance(velocity);
        //    }
        //    mSplineDistance = (int) (totalDistance * Math.Sign(velocity));
        //    mFinal = start + mSplineDistance;
        //    // Clamp to a valid final position
        //    if (mFinal < min) {
        //        adjustDuration(mStart, mFinal, min);
        //        mFinal = min;
        //    }
        //    if (mFinal > max) {
        //        adjustDuration(mStart, mFinal, max);
        //        mFinal = max;
        //    }
        //}


        private readonly IInterpolator mInterpolator;


        private Int32 mMode;

        //// The allowed overshot distance before boundary is reached.
        //private Int32 mOver;

        //// Distance to travel along spline animation
        //private Int32 mSplineDistance;

        //// Duration to complete spline component of animation
        //private Int32 mSplineDuration;

        //private Int32 mStart;

        //// Animation starting time, in system milliseconds
        //private Int64 mStartTime;

        //private Int32 mState = SPLINE;

        //// Initial velocity
        //private Int32 mVelocity;
    }
}
