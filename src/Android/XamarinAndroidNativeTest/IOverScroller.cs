using System;

namespace XamarinAndroidNativeTest
{
    public interface IOverScroller
    {
        Single CurrentYVelocity { get; }

        Single CurrVelocity { get; }

        Single CurrY { get; }

        Int32 FinalY { get; }

        Int32 StartY { get; }

        void abortAnimation();

        Boolean computeScrollOffset();

        void fling(Int32 startX,
                   Int32 startY,
                   Int32 velocityX,
                   Int32 velocityY,
                   Int32 minX,
                   Int32 maxX,
                   Int32 minY,
                   Int32 maxY,
                   Int32 overX,
                   Int32 overY);

        Boolean isFinished();

        Boolean springBack(Int32 startX,
                           Int32 startY,
                           Int32 minX,
                           Int32 maxX,
                           Int32 minY,
                           Int32 maxY);

        void startScroll(Int32 startX,
                         Int32 startY,
                         Int32 dx,
                         Int32 dy,
                         Int32 duration);

        void StartScroll(Int32 startX,
                         Int32 startY,
                         Int32 dx,
                         Int32 dy);
    }
}
