using System;
using System.IO;
using System.Windows.Forms;
using Das.OpenGL.Text.FreeType;
using Das.OpenGL.Windows;
using Das.Views;
using Das.Views.Core.Writing;
using Das.Views.Panels;
using Das.Views.Updaters;
using Das.Views.Winforms;
using Das.ViewsModels;

namespace Das.OpenGL
{
    public class GLBootStrapper : IBootStrapper
    {
        public StaScheduler Scheduler => _taskScheduler;
        private readonly StaScheduler _taskScheduler;
        private readonly GLWindowBuilder _windowBuilder;

        public GLBootStrapper(GLWindowBuilder windowBuilder)
        {
            _windowBuilder = windowBuilder;
            _taskScheduler = new StaScheduler("Das GL Thread");
        }

        public void Run<TViewModel>(TViewModel viewModel, IView view)
            where TViewModel : IViewModel
        {
            var window = _windowBuilder.Show(viewModel, view);
            Cook(window);
            Application.Run(window);
        }

        public static IFontProvider GetFontProvider(IGLContext context)
        {
            var fontFolder = new DirectoryInfo(Environment.GetFolderPath(
                Environment.SpecialFolder.Fonts));
            return new FreeTypeFontProvider(fontFolder, context);
        }

        public GLWindowsContext GetContext(IWindowsHost host) =>
            new GLWindowsContext(host, OpenGLVersion.OpenGL2_1, _windowBuilder);

        private void Cook(GLHostedElement element)
        {
            var context = GetContext(element);
            var fontProvider = GetFontProvider(context);

            var kit = new OpenGLRenderKit(fontProvider, context);

            var renderer = new GLFormViewRenderer(element, kit, context);
            var _ = new LoopViewUpdater(renderer, element, _taskScheduler);
        }
    }
}
