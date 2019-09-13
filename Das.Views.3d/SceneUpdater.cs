using System;
using System.Threading.Tasks;

namespace Das.Views.Extended
{
    public class SceneUpdater : BaseLoopUpdater
    {
        private readonly ISceneViewModel _viewModel;

        public SceneUpdater(ISceneViewModel viewModel, Int32 maxFramesPerSecond = 60)
            : base(maxFramesPerSecond)
        {
            _viewModel = viewModel;

            Task.Factory.StartNew(GameLoop, TaskCreationOptions.LongRunning);
        }

        protected override bool IsChanged => true;
        protected override void Update()
        {
            _viewModel.Update();
        }
    }
}
