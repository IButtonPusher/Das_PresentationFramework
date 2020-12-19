using System;
using System.IO;
using System.Windows.Forms;
using Das.OpenGL.Text.FreeType;
using Das.OpenGL.Windows;
using Das.Views;
using Das.Views.Core.Writing;
using Das.Views.Panels;
using Das.Views.Updaters;
using Das.Views.Windows;
using Das.Views.Styles;

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

        public void Run(IView view)
        {
            var window = _windowBuilder.Show(view);
            Cook(window);
            Application.Run(window);
        }

        public IVisualBootstrapper VisualBootstrapper => throw new NotImplementedException();

        public static IFontProvider GetFontProvider(IGLContext context)
        {
            var fontFolder = new DirectoryInfo(Environment.GetFolderPath(
                Environment.SpecialFolder.Fonts));
            return new FreeTypeFontProvider(fontFolder, context);
        }

        public GLWindowsContext GetContext(IWindowsHost host)
        {
            return new GLWindowsContext(host, OpenGLVersion.OpenGL2_1, 
                _windowBuilder, host.GraphicsDeviceContextPromise);
        }

        private void Cook(GLHostedElement element)
        {
            var context = GetContext(element);
            var fontProvider = GetFontProvider(context);
            var styleContext = new BaseStyleContext(DefaultStyle.Instance,
                new DefaultColorPalette());

            var kit = new OpenGLRenderKit(fontProvider, context, styleContext);

            var renderer = new GLFormViewRenderer(element, kit, context);
            var _ = new LoopViewUpdater(renderer, element, _taskScheduler);
        }
    }
}
