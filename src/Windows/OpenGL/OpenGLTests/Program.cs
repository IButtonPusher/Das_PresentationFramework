using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Das.Container;
using Das.OpenGL;
using Das.OpenGL.Windows;
using Das.Serializer;
using Das.Views.Construction;
using Das.Views.Construction.Styles;
using Das.Views.Extended;
using Das.Views.Layout;
using Das.Views.Styles;
using Das.Views.Styles.Construction;
using Das.Views.Templates;
using TestCommon;

namespace OpenGLTests
{
   internal static class Program
   {
      /// <summary>
      ///    The main entry point for the application.
      /// </summary>
      [STAThread]
      private static async Task Main(String[] args)
      {
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);

         if (args.Length > 0)
         {
            var fileName = args[0];

            if (File.Exists(fileName))
            {
               var lodr = new CoreFbxLoader();
               var fi = new FileInfo(fileName);
               var model = await lodr.LoadModelAsync(fi);
            }
         }

         //RunTest();

         Application.Run(new Form1());
      }

      private static void RunTest()
      {
         var serializer = new DasSerializer();

         var imageProvider = new TestImageProvider();

         var themeProvider = BaselineThemeProvider.Instance;
         var variableAccessor = new StyleVariableAccessor(themeProvider.ColorPalette);
         var inflator = new DefaultStyleInflater(serializer.TypeInferrer, variableAccessor);
         var styles = new VisualStyleProvider(inflator);

         var bootstrapper = new DefaultVisualBootstrapper(new BaseResolver(),
            themeProvider, serializer.TypeManipulator,
            new LayoutQueue(), new AppliedRuleBuilder(styles, new DeclarationWorker(),
               serializer.TypeManipulator));

         var viewInflater = new ViewInflater(bootstrapper, serializer, imageProvider);

         var visual = TestLauncher.GetTestCompanyTabsView(viewInflater, bootstrapper);

         var windowBuilder = new GLWindowBuilder("OpenGLSurface", bootstrapper);
         var boot = new GLBootStrapper(windowBuilder);

         boot.Run(visual);


         //var frm = windowBuilder.Show(visual);

         //Application.Run(frm);

         //var context = boot.GetContext(frm);

         //var fontProvider = GLBootStrapper.GetFontProvider(context);

         //var renderKit = new OpenGLRenderKit(fontProvider, context, themeProvider, 
         //   imageProvider);

         //return new TestLauncher(boot, renderKit, serializer.TypeInferrer);
      }
   }
}
