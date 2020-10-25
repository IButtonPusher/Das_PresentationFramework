using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Das.Views
{
    public abstract class BaseLoopUpdater : IDisposable
    {
        public BaseLoopUpdater(Int32 maxFramesPerSecond = 60)
        {
            _minDelay = 1000 / maxFramesPerSecond;
        }

        public virtual void Dispose()
        {
            _isDisposed = true;
        }

        protected abstract Boolean IsChanged { get; }

        protected async Task GameLoop()
        {
            Initialize();

            var swLast = new Stopwatch();
            swLast.Start();

            do
            {
                swLast.Restart();

                if (IsChanged)
                {
                    Update();
                    continue;
                }

                if (swLast.ElapsedMilliseconds >= _minDelay)
                    continue;

                var letsWait = _minDelay - (Int32) swLast.ElapsedMilliseconds;
                if (letsWait < 0)
                    continue;
                #if !NET40
                await Task.Delay(letsWait);
                #else
                System.Threading.Thread.Sleep(letsWait);
                #endif
            } while (!_isDisposed);
        }

        protected virtual void Initialize()
        {
        }

        protected abstract void Update();

        private readonly Int32 _minDelay;

        private Boolean _isDisposed;
    }
}