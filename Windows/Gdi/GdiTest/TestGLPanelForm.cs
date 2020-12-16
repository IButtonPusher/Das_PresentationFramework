using System;
using System.Windows.Forms;
using Das.Views.Panels;
using Das.Views.Styles;

namespace GdiTest
{
    public partial class TestGLPanelForm : Form
    {
        public TestGLPanelForm() 
        {
            var v = new View<Object>();
            var pnl = new TestOpenGLPanel(v, new BaseStyleContext(new DefaultStyle()));
            pnl.Dock = DockStyle.Fill;
            Controls.Add(pnl);
        }
    }
}
