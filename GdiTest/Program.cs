using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Das.Gdi;
using Das.OpenGL;
using Das.Views;
using Das.Views.Extended;
using Das.Views.Panels;
using TestCommon;
using ViewCompiler;
using Das.OpenGL.Windows;

namespace GdiTest
{
    static class Program
    {
        private static TestLauncher _testLauncher;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Application.Run(new TestGLPanelForm());

            _testLauncher = GetGdiLauncher();
            //_testLauncher = GetOpenGLLauncher();
            
            _testLauncher.MvvmTest().Wait();
            

            //_testLauncher.CubeTest().Wait();



            //            ZoomTest();
            //            DesignTest();
            //            MvvmTest();
            //            CubeTest().Wait();
        }

        // ReSharper disable once UnusedMember.Local
        private static TestLauncher GetGdiLauncher()
        {
            var provider = new GdiProvider();
            return new TestLauncher(provider);
        }

        // ReSharper disable once UnusedMember.Local
        private static TestLauncher GetOpenGLLauncher()
        {
            var windowBuilder = new GLWindowBuilder("OpenGLSurface");
            var provider = new GLBootStrapper(windowBuilder);
            return new TestLauncher(provider);
        }

        // ReSharper disable once UnusedMember.Local
        private static void DesignTest()
        {
            var file = new FileInfo("company\\EmployeesView.json");
            var form = Design(file);

            Application.Run(form);
        }

        // ReSharper disable once UnusedMember.Local
        private static void ZoomTest()
        {
            var file = new FileInfo("company\\ZoomTest.json");
            var form = Design(file);
            Application.Run(form);
        }


        private static DesignViewWindow Design(FileInfo file)
        {
            var provider = new DesignerProvider();
            var view = provider.Design(file);
            return view;
        }

        private static ViewWindow Show<TViewModel>(TViewModel viewModel, IView view)
            where TViewModel : IViewModel
        {
            var provider = new GdiProvider();
            return provider.Show(viewModel, view);
        }

        // ReSharper disable once UnusedMember.Local
        private static async Task CubeTest()
        {
            var viewProvider = new ViewProvider();
            var provider = new CubeSceneProvider(viewProvider);
            var vm = provider.BuildViewModel();
            var v = await provider.GetView();
            var _ = new SceneUpdater(vm, 15);
            var form = Show(vm, v);
            Application.Run(form);
        }
    }
}
