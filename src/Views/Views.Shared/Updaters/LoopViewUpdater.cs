using System;
using System.Threading;
using System.Threading.Tasks;
using Das.Views.Rendering;

#if !NET40
using TaskEx = System.Threading.Tasks.Task;
#endif

namespace Das.Views
{
    /// <summary>
    ///     Runs a 'game loop' that checks if the IViewHost has changes and redraws when it does
    /// </summary>
    public class LoopViewUpdater<TAsset> : BaseLoopUpdater
    {
        public LoopViewUpdater(IViewHost<TAsset> viewHost,
                               IRenderer<TAsset> renderer,
                               ILayoutQueue layoutQueue,
                               Int32 maxFramesPerSecond = 60)
            : base(maxFramesPerSecond)
        {
            _viewHost = viewHost;
            _renderer = renderer;
            _layoutQueue = layoutQueue;

            _viewHost.HostCreated += OnHostReady;
        }

        private Task OnHostReady()
        {
            _viewHost.HostCreated -= OnHostReady;
            Task.Factory.StartNew(GameLoop, TaskCreationOptions.LongRunning);
            return TaskEx.CompletedTask;
        }

        protected override Boolean IsChanged => _viewHost.IsChanged || _viewHost.StyleContext.IsChanged ||
                                                _layoutQueue.HasVisualsNeedingArrange;

        protected override Boolean Update()
        {
            var asset = _renderer.DoRender();
            if (asset == null)
                return false;

            _viewHost.Asset = asset;
            _viewHost.Invalidate();

            _viewHost.StyleContext.AcceptChanges();

            return true;
        }

        private readonly IRenderer<TAsset> _renderer;
        private readonly ILayoutQueue _layoutQueue;

        private readonly IViewHost<TAsset> _viewHost;
    }

    public class LoopViewUpdater : BaseLoopUpdater
    {
        public LoopViewUpdater(IRenderer renderer, IViewHost viewHost,
                               TaskScheduler taskScheduler, Int32 maxFramesPerSecond = 60)
            : base(maxFramesPerSecond)
        {
            _renderer = renderer;
            _viewHost = viewHost;

            Task.Factory.StartNew(GameLoop, CancellationToken.None,
                TaskCreationOptions.LongRunning, taskScheduler);
        }

        protected override Boolean IsChanged => _viewHost.IsChanged || _viewHost.StyleContext.IsChanged;

        protected override void Initialize()
        {
            _renderer.Initialize();
        }

        protected override Boolean Update()
        {
            _renderer.DoRender();
            _viewHost.AcceptChanges();

            return true;
        }

        private readonly IRenderer _renderer;
        private readonly IViewHost _viewHost;
    }
}