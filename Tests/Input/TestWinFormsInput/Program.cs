using System;
using System.Windows.Forms;
using Das.Gdi;
using TestCommon;
using ViewCompiler;

namespace TestWinFormsInput
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        // ReSharper disable once UnusedMember.Local
        private static TestLauncher GetGdiLauncher()
        {
            var gdi = new GdiProvider();
            var views = new ViewProvider();
            return new TestLauncher(gdi, views);
        }
    }
}
