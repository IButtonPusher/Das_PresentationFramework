using System.IO;
using System.Threading.Tasks;
using Das.Views;
using Das.Views.Extended;
using Das.Views.Panels;
using Das.ViewsModels;

namespace TestCommon
{
    public class TestLauncher
    {
        private readonly IBootStrapper _windowProvider;
        private readonly IViewProvider _viewProvider;
        private readonly ISingleThreadedInvoker _staInvoker;

        public TestLauncher(IBootStrapper windowProvider,
                            IViewProvider viewProvider,
                            ISingleThreadedInvoker staInvoker)
        {
            _windowProvider = windowProvider;
            _viewProvider = viewProvider;
            _staInvoker = staInvoker;
        }

        public async Task MvvmTest()
        {
            var file = new FileInfo("company\\EmployeesView.json");
            var view = await _viewProvider.GetView(file);

            var vm = new TestCompanyVm(_staInvoker);
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
