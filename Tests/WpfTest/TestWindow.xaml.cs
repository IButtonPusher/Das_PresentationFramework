using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using Windows.Shared;

namespace WpfTest
{
    /// <summary>
    /// Window->HwndHost (Win32Host)->HwndSource (WpfHost)
    ///
    ///Wpf hosting *win32* hosting wpf.... *gets windows messages
    /// 
    /// </summary>
    public partial class TestWindow : Window
    {
        private Win32Host _host;
        private readonly AutoResetEvent _resentEvent;
        private WpfHost _src;

        public TestWindow()
        {
            _resentEvent = new AutoResetEvent(false);
            InitializeComponent();

//            var lbl = new Label {Content = "hello world"};
//            var src = new WpfHost(lbl);
//
//            _host = new Win32Host(src, lbl);

            //_addinPanel.Child = _host;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            var thread = new Thread(MessagePumpThread)
            {
                IsBackground = true
            };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Name = "Map UI Thread";
            thread.Start();

            _resentEvent.WaitOne();

            _host = new Win32Host(_src);
            _addinPanel.Child = _host;
        }

        private void MessagePumpThread()
        {
            var lbl = new Button { Content = "hello world" };
            lbl.Click += (o, e) => { MessageBox.Show("hello there");};
            _src = new WpfHost(lbl);

            _resentEvent.Set();

            Dispatcher.Run();
        }

        private void OnLoaded(Object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            var wmsg = (MessageTypes) msg;
            Debug.WriteLine("main " + wmsg);
            return IntPtr.Zero;
        }
    }
}
