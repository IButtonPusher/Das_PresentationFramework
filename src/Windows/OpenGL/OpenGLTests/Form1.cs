using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenGLTests.Samples;

namespace OpenGLTests
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            var ptr = Handle;

            Task.Run(() => BobRunner(ptr));

        }

        private void BobRunner(IntPtr ptr)
        {
            //var bob = new HelloTriangle(this, ptr);
            var bob = new HelloShaders(this, ptr);

            while (true)
            {
                bob.Paint();
                Thread.Sleep(5);
            }

            // ReSharper disable once FunctionNeverReturns
        }

        protected override void OnPaint(PaintEventArgs e)
        {
         //   _bob.Paint();
        }
    }
}
