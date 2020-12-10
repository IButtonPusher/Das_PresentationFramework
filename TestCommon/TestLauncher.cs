using System.IO;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views;
using Das.Views.Extended;
using Das.Views.Panels;
using Das.Views.Mvvm;

namespace TestCommon
{
    public class TestLauncher
    {
        public IBootStrapper BootStrapper { get; }

        public IRenderKit RenderKit { get; }

        public ITypeInferrer TypeInferrer { get; }


        private readonly IViewProvider _viewProvider;

        public TestLauncher(IBootStrapper bootStrapper,
                            IViewProvider viewProvider,
                                IRenderKit renderKit, 
                            ITypeInferrer typeInferrer)
        {
            BootStrapper = bootStrapper;
            RenderKit = renderKit;
            TypeInferrer = typeInferrer;
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

        public virtual void Run(IViewModel vm, IView view)
        {
            BootStrapper.Run(vm, view);
        }

        // ReSharper disable once UnusedMember.Global
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
