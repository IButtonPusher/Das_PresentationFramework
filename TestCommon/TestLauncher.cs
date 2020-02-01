using System.IO;
using System.Threading.Tasks;
using Das.Views;
using Das.Views.Extended;
using Das.Views.Panels;

namespace TestCommon
{
    public class TestLauncher
    {
        private readonly IBootStrapper _windowProvider;
        private readonly IViewProvider _viewProvider;

        public TestLauncher(IBootStrapper windowProvider, IViewProvider viewProvider)
        {
            _windowProvider = windowProvider;
            _viewProvider = viewProvider;
        }

        public async Task MvvmTest()
        {
            var file = new FileInfo("company\\EmployeesView.json");
            var view = await _viewProvider.GetView(file);

            var vm = new TestCompanyVm();
            var _ = new SceneUpdater(vm, 50);
            Run(vm, view);
        }

        protected virtual void Run(IViewModel vm, IView view)
        {
            _windowProvider.Run(vm, view);
        }

        public async Task CubeTest()
        {
            var provider = new CubeSceneProvider(_viewProvider);
            var vm = provider.BuildViewModel();
            var v = await provider.GetView();
            var _ = new SceneUpdater(vm, 50);
            Run(vm, v);
        }
    }
}
