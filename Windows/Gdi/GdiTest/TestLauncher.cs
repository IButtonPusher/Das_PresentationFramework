using System.IO;
using System.Threading.Tasks;
using Das.Views;
using Das.Views.Extended;
using Das.Views.Panels;
using TestCommon;
using ViewCompiler;

namespace GdiTest
{
    public class TestLauncher
    {
        private readonly IBootStrapper _windowProvider;

        public TestLauncher(IBootStrapper windowProvider)
        {
            _windowProvider = windowProvider;
        }

        public async Task MvvmTest()
        {
            var file = new FileInfo("company\\EmployeesView.json");
            var provider = new ViewProvider();
            var view = await provider.GetView(file);

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
            var viewProvider = new ViewProvider();
            var provider = new CubeSceneProvider(viewProvider);
            var vm = provider.BuildViewModel();
            var v = await provider.GetView();
            var _ = new SceneUpdater(vm, 50);
            Run(vm, v);
        }
    }
}
