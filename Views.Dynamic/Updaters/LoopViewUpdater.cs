using System;
using System.Threading;
using System.Threading.Tasks;
using Das.Views.Rendering;


namespace Das.Views
{
    /// <summary>
    /// Runs a 'game loop' that checks if the IViewHost has changes and redraws when it does
    /// </summary>
    public class LoopViewUpdater<TAsset> : BaseLoopUpdater
    {
        public LoopViewUpdater(IViewHost<TAsset> viewHost,
            IRenderer<TAsset> renderer, Int32 maxFramesPerSecond = 60)
            :base(maxFramesPerSecond)
        {
            _viewHost = viewHost;
            _renderer = renderer;
            

            Task.Factory.StartNew(GameLoop, TaskCreationOptions.LongRunning);
        }

        private readonly IViewHost<TAsset> _viewHost;
        private readonly IRenderer<TAsset> _renderer;

        protected override Boolean IsChanged => _viewHost.IsChanged;

        protected override void Update()
        {
            var asset = _renderer.DoRender();
            _viewHost.Asset = asset;
            _viewHost.Invalidate();

            _viewHost.AcceptChanges();
        }
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
        
        private readonly IRenderer _renderer;
        private readonly IViewHost _viewHost;

        protected override Boolean IsChanged => _viewHost.IsChanged;

        protected override void Initialize()
        {
            _renderer.Initialize();
        }

        protected override void Update()
        {
            _renderer.DoRender();
            _viewHost.AcceptChanges();
        }
    }
}