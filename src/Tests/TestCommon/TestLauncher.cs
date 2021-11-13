using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views;
using Das.Views.Construction;
using Das.Views.DataBinding;
using Das.Views.Panels;
using TestCommon.Company;

namespace TestCommon
{
    public class TestLauncher
    {
        public IBootStrapper BootStrapper { get; }

        public IRenderKit RenderKit { get; }

        public ITypeInferrer TypeInferrer { get; }


        //private readonly IViewProvider _viewProvider;

        public TestLauncher(IBootStrapper bootStrapper,
                            //IViewProvider viewProvider,
                                IRenderKit renderKit, 
                            ITypeInferrer typeInferrer)
        {
            BootStrapper = bootStrapper;
            RenderKit = renderKit;
            TypeInferrer = typeInferrer;
            //_viewProvider = viewProvider;
        }

        public static IView GetTestCompanyTabsView(IViewInflater inflater,
                                                   IVisualBootstrapper visualBootstrapper)
        {
           //var inflater = testLauncher.RenderKit.ViewInflater;
            
           //var inflator = new ViewInflater(testLauncher.RenderKit.VisualBootstrapper,
           //    testLauncher.TypeInferrer);

           var file = new FileInfo(
              Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                 "company", "TestCompanyTabs.xml"));

           var xml = File.ReadAllText(file.FullName);
           var view = inflater.InflateXmlAsync<IBindableElement>(xml).Result;

           ICompanyViewModel vm = new TestCompanyVm();
           vm.SelectedEmployee = vm.Employees.FirstOrDefault();

           view.DataContext = vm;

           //return view;
           return new View(visualBootstrapper)
           {
              Content = view
           };
        }

        //public async Task MvvmTest()
        //{
        //    var file = new FileInfo("company\\EmployeesView.json");
        //    var view = await _viewProvider.GetView(file);

        //    if (!(view is IView valid))
        //        throw new InvalidCastException();

        //    var vm = new TestCompanyVm();
        //    var _ = new SceneUpdater(vm, 50);
        //    Run(vm, valid);
        //}

        public virtual void Run<TViewModel>(TViewModel vm, 
                                            IView view)
        {
            BootStrapper.Run(//vm, 
                view);
        }

        // ReSharper disable once UnusedMember.Global
        //public async Task CubeTest()
        //{
        //    var provider = new CubeSceneProvider(_viewProvider);
        //    var vm = provider.BuildViewModel();
        //    var v = await provider.GetView();
        //    var _ = new SceneUpdater(vm, 50);
        //    Run(vm, v);
        //}
    }
}
