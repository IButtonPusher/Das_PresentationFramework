using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GdiTest
{
    public partial class TestBrowser : Form
    {
        private WebBrowser _browser;

        public TestBrowser()
        {
            InitializeComponent();

            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw,
                true);

            _browser = new WebBrowser();
            _browser.Parent = this;
            _browser.Dock = DockStyle.Fill;
         

            _browser.Navigate("www.aol.com");
        }

        
    }
}
