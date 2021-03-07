using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Android.Content;
using Android.Hardware;
using Android.Views;
using Android.Views.Animations;

namespace XamarinAndroidNativeTest
{
    public class SplineOverScrollerToo
    {
        //static SplineOverScrollerToo()
        //{
        //    var x_min = 0.0f;
        //    var y_min = 0.0f;
        //    for (var i = 0; i < NB_SAMPLES; i++)
        //    {
        //        var alpha = (Single) i / NB_SAMPLES;
        //        var x_max = 1.0f;
        //        Single x, tx, coef;
        //        while (true)
        //        {
        //            x = x_min + (x_max - x_min) / 2.0f;
        //            coef = 3.0f * x * (1.0f - x);
        //            tx = coef * ((1.0f - x) * P1 + x * P2) + x * x * x;
        //            if (Math.Abs(tx - alpha) < 1E-5) break;
        //            if (tx > alpha) x_max = x;
        //            else x_min = x;
        //        }

        //        SPLINE_POSITION[i] = coef * ((1.0f - x) * START_TENSION + x) + x * x * x;
        //        var y_max = 1.0f;
        //        Single y, dy;
        //        while (true)
        //        {
        //            y = y_min + (y_max - y_min) / 2.0f;
        //            coef = 3.0f * y * (1.0f - y);
        //            dy = coef * ((1.0f - y) * START_TENSION + y) + y * y * y;
        //            if (Math.Abs(dy - alpha) < 1E-5) break;
        //            if (dy > alpha) y_max = y;
        //            else y_min = y;
        //        }

        //        SPLINE_TIME[i] = coef * ((1.0f - y) * P1 + y * P2) + y * y * y;
        //    }

        //    SPLINE_POSITION[NB_SAMPLES] = SPLINE_TIME[NB_SAMPLES] = 1.0f;
        //}

        static SplineOverScrollerToo()
        {
            var x_min = 0.0f;
            var y_min = 0.0f;
            for (var i = 0; i < 100; i++)
            {
                Single x;
                Single coef;
                Single y;
                var alpha = i / 100.0f;
                var x_max = END_TENSION;
                while (true)
                {
                    x = x_min + (x_max - x_min) / 2.0f;
                    coef = 3.0f * x * (END_TENSION - x);
                    var tx = ((END_TENSION - x) * P1 + P2 * x) * coef + x * x * x;
                    if (Math.Abs(tx - alpha) < 1.0E-5d)
                        break;
                    if (tx > alpha)
                        x_max = x;
                    else
                        x_min = x;
                }

                SPLINE_POSITION[i] = ((END_TENSION - x) * 0.5f + x) * coef + x * x * x;
                var y_max = END_TENSION;
                while (true)
                {
                    y = y_min + (y_max - y_min) / 2.0f;
                    coef = 3.0f * y * (END_TENSION - y);
                    var dy = ((END_TENSION - y) * 0.5f + y) * coef + y * y * y;
                    if (Math.Abs(dy - alpha) < 1.0E-5d)
                        break;
                    if (dy > alpha)
                        y_max = y;
                    else
                        y_min = y;
                }

                SPLINE_TIME[i] = ((END_TENSION - y) * P1 + P2 * y) * coef + y * y * y;
            }

            var fArr = SPLINE_POSITION;
            SPLINE_TIME[100] = END_TENSION;
            fArr[100] = END_TENSION;
        }

        public SplineOverScrollerToo(Context context,
                                     String name)
        {
            _name = name;
            mFinished = true;
            var ppi = context.Resources.DisplayMetrics.Density * 160.0f;
            mPhysicalCoeff = SensorManager.GravityEarth // g (m/s^2)
                             * 39.37f // inch/meter
                             * ppi
                             * 0.84f; // look and feel tuning

            var coeff2 = (386.0878f * (context.Resources.DisplayMetrics.Density * 160.0f)) * 0.84f;
        }

        public Int32 mCurrentPosition { get; private set; }

        //set
        //{
        //    if (_mCurrentPosition == value)
        //        return;
        //    var delta = value - _mCurrentPosition;
        //    if (Math.Abs(delta) > 500)
        //    {}
        //    BadLog.WriteLine("SplineOverScroller2 (" + _name + ") set mCurrentPosition = " + 
        //                     value + " => +/- " + delta);
        //    _mCurrentPosition = value;
        //}
        public Single mCurrVelocity { get; private set; }

        public Boolean continueWhenFinished()
        {
            switch (mState)
            {
                case SPLINE:
                    // Duration from start to null velocity
                    if (mDuration < mSplineDuration)
                    {
                        // If the animation was clamped, we reached the edge
                        mStart = mFinal;
                        UpdateCurrentPosition(mStart);
                        //mCurrentPosition = mStart = mFinal;
                        // TODO Better compute speed when edge was reached
                        mVelocity = (Int32) mCurrVelocity;
                        mDeceleration = getDeceleration(mVelocity);
                        mStartTime += mDuration;
                        onEdgeReached();
                    }
                    else // Normal stop, no need to continue
                        return false;

                    break;
                case BALLISTIC:
                    mStartTime += mDuration;
                    startSpringback(mFinal, mStart, 0);
                    break;
                case CUBIC:
                    return false;
            }

            update();
            return true;
        }

        public void finish()
        {
            //if (mFinished)
            //    return;

            BadLog.WriteLine("SplitOverScroller2 " + _name + " finish()");
            UpdateCurrentPosition(mFinal);
            //mCurrentPosition = mFinal;
            // Not reset since WebView relies on this value for fast fling.
            // TODO: restore when WebView uses the fast fling implemented in this class.
            // mCurrVelocity = 0.0f;
            mFinished = true;
        }

        public void fling(Int32 start,
                          Int32 velocity,
                          Int32 min,
                          Int32 max,
                          Int32 over)
        {
            mOver = over;
            mFinished = false;
            mVelocity = velocity;
            UpdateCurrentVelocity(velocity);
            mDuration = mSplineDuration = 0;
            mStartTime = AnimationUtils.CurrentAnimationTimeMillis();

            mStart = start;
            UpdateCurrentPosition(start);
            //mCurrentPosition = mStart = start;
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
            // Clamp to a valid position
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
            //else
            //{
            //    mFinished = true;
            //}
        }

        public Boolean springback(Int32 start,
                                  Int32 min,
                                  Int32 max)
        {
            mFinished = true;
            //mCurrentPosition = mStart = mFinal = start;
            mStart = mFinal = start;
            UpdateCurrentPosition(start);
            mVelocity = 0;
            mStartTime = AnimationUtils.CurrentAnimationTimeMillis();
            mDuration = 0;
            if (start < min)
                startSpringback(start, min, 0);
            else if (start > max)
                startSpringback(start, max, 0);
            return !mFinished;
        }

        public void startScroll(Int32 start,
                                Int32 distance,
                                Int32 duration)
        {
            mFinished = false;

            mStart = start;
            UpdateCurrentPosition(start);
            mFinal = start + distance;
            mStartTime = AnimationUtils.CurrentAnimationTimeMillis();
            mDuration = duration;
            // Unused
            mDeceleration = 0.0f;
            mVelocity = 0;
        }

        /*
         * Update the current position and velocity for current time. Returns
         * true if update has been done and false if animation duration has been
         * reached.
         */
        public Boolean update()
        {
            //BadLog.WriteLine("Updating spline scroller via " + coller);

            //if (mFinished)
            //    return false;

            var time = AnimationUtils.CurrentAnimationTimeMillis();
            var currentTime = time - mStartTime;
            if (currentTime == 0) // Skip work but report that we're still going if we have a nonzero duration.
                return mDuration > 0;
            if (currentTime > mDuration)
                return false;
            var distance = 0.0;
            switch (mState)
            {
                case SPLINE:
                {
                    //var t = (Single) currentTime / mSplineDuration;
                    var t = currentTime / (Single) mSplineDuration;
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
                    UpdateCurrentVelocity(velocityCoef * mSplineDistance / mSplineDuration * 1000.0f,
                        "update where index = " + index + " t = " + t);
                    break;
                }
                case BALLISTIC:
                {
                    var t = currentTime / 1000.0f;
                    UpdateCurrentVelocity(mVelocity + mDeceleration * t);

                    distance = mVelocity * t + mDeceleration * t * t / 2.0f;
                    break;
                }
                case CUBIC:
                {
                    var t = (Single) currentTime / mDuration;
                    var t2 = t * t;
                    Single sign = Math.Sign(mVelocity);
                    distance = sign * mOver * (3.0f * t2 - 2.0f * t * t2);
                    UpdateCurrentVelocity(sign * mOver * 6.0f * (-t + t2));
                    break;
                }
            }

            UpdateCurrentPosition(mStart + (Int32) Math.Round(distance));
            return true;
        }

        public void updateScroll(Single q)
        {
            UpdateCurrentPosition(Convert.ToInt32(mStart + Math.Round(q * (mFinal - mStart))));
        }

        /*
         * Modifies mDuration to the duration it takes to get from start to newFinal using the
         * spline interpolation. The previous duration was needed to get to oldFinal.
         */
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
                mDuration = (Int32) (mDuration * timeCoef);
                //mDuration *= timeCoef;
            }
        }

        private void extendDuration(Int32 extend)
        {
            var time = AnimationUtils.CurrentAnimationTimeMillis();
            var elapsedTime = (Int32) (time - mStartTime);
            mDuration = elapsedTime + extend;
            mFinished = false;
        }

        private void fitOnBounceCurve(Int32 start,
                                      Int32 end,
                                      Int32 velocity)
        {
            // Simulate a bounce that started from edge
            var durationToApex = -velocity / mDeceleration;
            // The float cast below is necessary to avoid integer overflow.
            var velocitySquared = (Single) velocity * velocity;
            var distanceToApex = velocitySquared / 2.0f / Math.Abs(mDeceleration);
            Single distanceToEdge = Math.Abs(end - start);
            var totalDuration = (Single) Math.Sqrt(
                2.0 * (distanceToApex + distanceToEdge) / Math.Abs(mDeceleration));
            mStartTime -= (Int32) (1000.0f * (totalDuration - durationToApex));
            //mCurrentPosition = mStart = end;
            mStart = end;
            UpdateCurrentPosition(end);
            mVelocity = (Int32) (-mDeceleration * totalDuration);
        }

        /*
         * Get a signed deceleration that will reduce the velocity.
         */
        private static Single getDeceleration(Int32 velocity)
        {
            return velocity > 0 ? -GRAVITY : GRAVITY;
        }

        private Double getSplineDeceleration(Int32 velocity)
        {
            var gfd = Math.Log(INFLEXION * Math.Abs(velocity) / (mFlingFriction * mPhysicalCoeff));
            return gfd;
        }

        private Double getSplineFlingDistance(Int32 velocity)
        {
            var l = getSplineDeceleration(velocity);
            var decelMinusOne = DECELERATION_RATE - 1.0;
            var res = mFlingFriction * mPhysicalCoeff * Math.Exp(DECELERATION_RATE / decelMinusOne * l);
            //var res2 = getSplineFlingDistance2(velocity);
            return res;
        }

        //private double getSplineFlingDistance2(int velocity) {
        //    return mFlingFriction * mPhysicalCoeff * Math.Exp(
        //        (DECELERATION_RATE / (DECELERATION_RATE - 1.0d)) * 
        //        getSplineDeceleration(velocity));
        //}

        /* Returns the duration, expressed in milliseconds */
        private Int32 getSplineFlingDuration(Int32 velocity)
        {
            var l = getSplineDeceleration(velocity);
            var decelMinusOne = DECELERATION_RATE - 1.0;
            var omg = 1000.0 * Math.Exp(l / decelMinusOne);


            //return Convert.ToInt32(omg);
            return Convert.ToInt32(omg * 1.2);
        }

        //private void notifyEdgeReached(Int32 start,
        //                               Int32 end,
        //                               Int32 over)
        //{
        //    // mState is used to detect successive notifications 
        //    if (mState == SPLINE)
        //    {
        //        mOver = over;
        //        mStartTime = AnimationUtils.CurrentAnimationTimeMillis();
        //        // We were in fling/scroll mode before: current velocity is such that distance to
        //        // edge is increasing. This ensures that startAfterEdge will not start a new fling.
        //        startAfterEdge(start, end, end, (Int32) mCurrVelocity);
        //    }
        //}

        private void onEdgeReached()
        {
            // mStart, mVelocity and mStartTime were adjusted to their values when edge was reached.
            // The float cast below is necessary to avoid integer overflow.
            var velocitySquared = (Single) mVelocity * mVelocity;
            var distance = velocitySquared / (2.0f * Math.Abs(mDeceleration));
            Single sign = Math.Sign(mVelocity);
            if (distance > mOver)
            {
                // Default deceleration is not sufficient to slow us down before boundary
                mDeceleration = -sign * velocitySquared / (2.0f * mOver);
                distance = mOver;
            }

            mOver = (Int32) distance;
            mState = BALLISTIC;
            mFinal = mStart + (Int32) (mVelocity > 0 ? distance : -distance);
            mDuration = -(Int32) (1000.0f * mVelocity / mDeceleration);
        }

        private void setFinalPosition(Int32 position)
        {
            mFinal = position;
            mFinished = false;
        }

        private void setFriction(Single friction)
        {
            mFlingFriction = friction;
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
            if (keepIncreasing) // Will result in a bounce or a to_boundary depending on velocity.
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
            UpdateCurrentPosition(start);
            mFinal = end;
            var delta = start - end;
            mDeceleration = getDeceleration(delta);
            // TODO take velocity into account
            mVelocity = -delta; // only sign is used
            mOver = Math.Abs(delta);
            mDuration = (Int32) (1000.0 * Math.Sqrt(-2.0 * delta / mDeceleration));
        }

        private void UpdateCurrentPosition(Int32 value,
                                           [CallerMemberName] String? wotDo = null)
        {
            if (mCurrentPosition == value)
                return;

            var delta = value - mCurrentPosition;

            if (Math.Abs(delta) > 500)
            {
            }

            BadLog.WriteLine("SplineOverScroller2 (" + _name + ") set mCurrentPosition = " +
                             value + " => +/- " + delta + " by " + wotDo);
            mCurrentPosition = value;
        }

        private void UpdateCurrentVelocity(Single value,
                                           [CallerMemberName] String? wotDo = null)
        {
            if (Equals(mCurrVelocity, value))
                return;

            BadLog.WriteLine("mCurrVelocity " + mCurrVelocity + " => " + value + " by " + wotDo +
                             " state: " + mState);

            mCurrVelocity = value;
        }

        private const Int32 SPLINE = 0;
        private const Int32 CUBIC = 1;

        private const Int32 BALLISTIC = 2;

        // Constant gravity value, used in the deceleration phase.
        private static readonly Single GRAVITY = 2000.0f;
        private static readonly Single DECELERATION_RATE = (Single) (Math.Log(0.78) / Math.Log(0.9));
        private static readonly Single INFLEXION = 0.35f; // Tension lines cross at (INFLEXION, 1)

        private const Single START_TENSION = 0.5f;
        private const Single END_TENSION = 1.0f;

        private static readonly Single P1 = START_TENSION * INFLEXION;
        private static readonly Single P2 = 1.0f - END_TENSION * (1.0f - INFLEXION);

        private const Int32 NB_SAMPLES = 100;
        private static readonly Single[] SPLINE_POSITION = new Single[NB_SAMPLES + 1];
        private static readonly Single[] SPLINE_TIME = new Single[NB_SAMPLES + 1];
        private readonly String _name;

        // A context-specific coefficient adjusted to physical values.
        private readonly Single mPhysicalCoeff;


        // Constant current deceleration
        private Single mDeceleration;

        // Animation duration, in milliseconds
        public Int32 mDuration;

        // Final position
        public Int32 mFinal;

        // Whether the animation is currently in progress
        public Boolean mFinished;

        //private Int32 _currentFlingStarted;

        // Fling friction
        private Single mFlingFriction = ViewConfiguration.ScrollFriction; // * 0.67f;

        // The allowed overshot distance before boundary is reached.
        private Int32 mOver;

        // Distance to travel along spline animation
        private Int32 mSplineDistance;

        // Duration to complete spline component of animation
        private Int32 mSplineDuration;

        // Initial position
        public Int32 mStart;

        // Animation starting time, in system milliseconds
        public Int64 mStartTime;

        // Current state of the animation.
        private Int32 mState = SPLINE;

        // Initial velocity
        private Int32 mVelocity;
    }
}
