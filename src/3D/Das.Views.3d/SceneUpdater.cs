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

        protected override Boolean IsChanged => true;
        protected override Boolean Update()
        {
            _viewModel.Update();
            return true;
        }
    }
}
