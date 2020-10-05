using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            var bob = new HelloTriangle(this, ptr);

            while (true)
            {
                bob.Paint();
                Thread.Sleep(5);
            }

        }

        protected override void OnPaint(PaintEventArgs e)
        {
         //   _bob.Paint();
        }
    }
}
