using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Das.Gdi;
using Das.Serializer;
//using Das.OpenGL;
using Das.Views.Extended;
using Das.Views.Panels;
using TestCommon;
using ViewCompiler;
//using Das.OpenGL.Windows;
using Das.Views;
using Das.Views.DataBinding;
using Das.Views.Mvvm;
using TestCommon.Company;

namespace GdiTest
{
    static class Program
    {
        //private static TestLauncher _testLauncher;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            //Application.Run(new TestBrowser());

//            new StaScheduler("GDI Test");

            var _testLauncher = GetGdiLauncher();
            
           
            

            var view = GetTestCompanyTabsView(_testLauncher);

            //_testLauncher.RenderKit.

            //_testLauncher = GetOpenGLLauncher();

            ICompanyViewModel vm = new TestCompanyVm();
            vm.SelectedEmployee = vm.Employees.FirstOrDefault();

            view.DataContext = vm;
            //view.SetDataContext(vm);

            //_testLauncher.r

            //var view = new TestTabControl(_testLauncher.BootStrapper.VisualBootstrapper);
            //view.SetDataContext(vm);

            _testLauncher.Run(vm, view);

            //_testLauncher.MvvmTest().Wait();


          //  ZoomTest();
        }

        private static IView GetTestCompanyTabsView(TestLauncher testLauncher)
        {
            var inflater = testLauncher.RenderKit.ViewInflater;
            
            //var inflator = new ViewInflater(testLauncher.RenderKit.VisualBootstrapper,
            //    testLauncher.TypeInferrer);

            var file = new FileInfo(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "company", "TestCompanyTabs.xml"));

            var xml = File.ReadAllText(file.FullName);
            var view = inflater.InflateXml<IBindableElement>(xml);

            //return view;
            return new Das.Views.Panels.View(testLauncher.RenderKit.VisualBootstrapper)
            {
                Content = view
            };
        }


        // ReSharper disable once UnusedMember.Local
        private static TestLauncher GetGdiLauncher()
        {
            var srl = new DasSerializer();
            
            var boot = new GdiProvider();
            return new TestLauncher(boot, boot.RenderKit,
                srl.TypeInferrer);
            
            //var viewProvider = new ViewProvider();
            //return new TestLauncher(viewProvider, viewProvider, viewProvider.RenderKit,
            //    viewProvider.TypeInferrer);
        }

        // ReSharper disable once UnusedMember.Local
        //private static TestLauncher GetOpenGLLauncher()
        //{
        //    var windowBuilder = new GLWindowBuilder("OpenGLSurface");
        //    var boot = new GLBootStrapper(windowBuilder);
        //    var viewProvider = new ViewProvider();
        //    return new TestLauncher(boot,viewProvider, viewProvider.RenderKit,
        //        viewProvider.TypeInferrer);
        //}

        // ReSharper disable once UnusedMember.Local
        //private static void DesignTest()
        //{
        //    var file = new FileInfo("company\\EmployeesView.json");
        //    var form = Design(file);

        //    Application.Run(form);
        //}

        // ReSharper disable once UnusedMember.Local
        //private static void ZoomTest()
        //{
        //    var file = new FileInfo("company\\ZoomTest.json");
        //    var form = Design(file);
        //    Application.Run(form);
        //}


        //private static DesignViewWindow Design(FileInfo file)
        //{
        //    var provider = new DesignerProvider();
        //    var view = provider.Design(file);
        //    return view;
        //}

        private static Form Show<TViewModel>(TViewModel viewModel, 
                                             IBindableElement view)
            where TViewModel : IViewModel
        {
            var provider = new GdiProvider();
            return provider.Show(viewModel, view);
        }

        // ReSharper disable once UnusedMember.Local
        //private static async Task CubeTest()
        //{
        //    var viewProvider = new ViewProvider();
        //    var provider = new CubeSceneProvider(viewProvider);
        //    var vm = provider.BuildViewModel();
        //    var v = await provider.GetView();
        //    var _ = new SceneUpdater(vm, 15);
        //    var form = Show(vm, v);
        //    Application.Run(form);
        //}
    }
}
