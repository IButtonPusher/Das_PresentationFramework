using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Windows.Shared;
using Windows.Shared.Messages;
using MSG = Windows.Shared.Messages.MSG;

//using Windows.Shared.Messages;

namespace WpfTest
{
    public partial class MainWindow
    {
        private WindowsMessageRouter _messageRouter;
        private IntPtr _handle;
        IntPtr _hook;
        CBTProc _callback;
        private Thread _thread;
        private UInt32 _mainThreadId;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += ImLoaded;

            MouseDown += MouseWentDown;

            
        }

        private struct POINTSTRUCT
        {
            public int x;
            public int y;

            public POINTSTRUCT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll", EntryPoint = "WindowFromPoint", CharSet = CharSet.Auto)]
        private static extern IntPtr IntWindowFromPoint(POINTSTRUCT pt);

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            var screenPoint = PointToScreen(new Point(0, 0));
            var ps = new POINTSTRUCT(Convert.ToInt32(screenPoint.X),
                Convert.ToInt32(screenPoint.Y));
            var handle = IntWindowFromPoint(ps);

            ComponentDispatcher.ThreadFilterMessage += OnThreadFilter;
        }

        private void OnThreadFilter(ref System.Windows.Interop.MSG msg, ref Boolean handled)
        {
            MessageTypes wmsg = (MessageTypes)msg.message;

            //if (msg.message == 513)
            //Debug.WriteLine("Thread filtering " + wmsg);
        }


        private void MouseWentDown(Object sender, MouseButtonEventArgs e)
        {
            //Debug.WriteLine("wpf says mouse went down");
        }

        private void OnTick(Object sender, EventArgs e)
        {
            MSG nomsg = default;
            _messageRouter.GetMessage(ref nomsg, _handle, 0, 0);

            var wmsg = (MessageTypes) nomsg.message;
            //if (wmsg == MessageTypes.WM_LBUTTONDOWN)
                //Debug.WriteLine("Got msg: mouse down");
        }

        private IntPtr CallBack(int code, IntPtr wParam, IntPtr lParam)
        {
            MessageTypes wmsg = (MessageTypes)wParam;
            //Debug.WriteLine("000 " + wmsg );
            //var wind = Window.GetWindow(this);
            if (code == 513)
            { }

//            if (_handle == null)
//            {
//                _handle = (new WindowInteropHelper(wind)).EnsureHandle();
//            }
            var mouseInfo = Marshal.PtrToStructure<MOUSEHOOKSTRUCT>(lParam);
//            AddStatusMsg($"{code} wParam {wParam} lParam {lParam} {mouseInfo}");
//            if (_chkBox.IsChecked == true)
//            {
//                return new IntPtr(1); // non-zero indicates don't pass to target
//            }
            return CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        }

        public delegate IntPtr CBTProc(int code, IntPtr wParam, IntPtr lParam);


        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr hookPtr, int nCode, IntPtr wordParam, IntPtr longParam);


        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(HookType hookType, CBTProc hookProc, IntPtr instancePtr, uint threadID);

        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            MessageTypes wmsg = (MessageTypes) msg;
            //Debug.WriteLine("hooked " + wmsg);

            if (msg == 513)
                handled = true;

//            var message = (WindowMessage)msg;
//            var subCode = (WindowMessageParameter)wParam.ToInt32();
//
//            if (message == WindowMessage.WM_POWERBROADCAST)
//            {
//                if (subCode == WindowMessageParameter.PBT_APMRESUMEAUTOMATIC)
//                {
//                    // handle suspend resumed event
//                }
//            }

            return IntPtr.Zero;
        }

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        private void HookBuilder(object o)
        {
            var objCreated = (ManualResetEventSlim )o;

            
            
            
            
            

            Hook.CreateHook(KeyHandler, _mainThreadId);

            _callback = CallBack;

            uint threadId = GetCurrentThreadId();

            _hook = SetWindowsHookEx(
                HookType.WH_MOUSE,
                _callback,
                instancePtr: IntPtr.Zero,
                threadId);

            objCreated.Set();
            System.Windows.Threading.Dispatcher.Run();


        }

        private async void ImLoaded(Object sender, RoutedEventArgs e)
        {
            _handle = Process.GetCurrentProcess().MainWindowHandle;
            var helper = new WindowInteropHelper(this);
            var rly = helper.Handle;

            _mainThreadId = GetCurrentThreadId();

            //HookBuilder();
            using (var objCreated = new ManualResetEventSlim(false))
            {
                _thread = new Thread(HookBuilder);
                _thread.SetApartmentState(ApartmentState.STA);
                _thread.IsBackground = true;
                _thread.Start(objCreated);

                objCreated.Wait();
            }


            var hwndSource = PresentationSource.FromVisual(this) as HwndSource;
                if (hwndSource != null)
                {
                    hwndSource.AddHook(WndProc);
                }
            

            _messageRouter = new WindowsMessageRouter(_handle);

            var t = new Thread(Ticking);
            t.Start();

            Loaded += ImLoaded;
//            var tmr = new DispatcherTimer();
//            tmr.Interval = TimeSpan.FromMilliseconds(100);
//            tmr.Tick += OnTick;
//            tmr.Start();
        }

        private void Ticking()
        {
            Thread.CurrentThread.IsBackground = true;
            while (true)
            {
                Thread.Sleep(10);
                MSG nomsg = default;
                _messageRouter.GetMessage(ref nomsg, _handle, 0, 0);

                var wmsg = (MessageTypes)nomsg.message;
                //if (wmsg == MessageTypes.WM_LBUTTONDOWN)
                    //Debug.WriteLine("Got msg: mouse down");
            }
        }

        public static void KeyHandler(IntPtr wParam, IntPtr lParam)
        {
            Debug.WriteLine("hmoney w " + wParam + " l " + lParam );
            int key = Marshal.ReadInt32(lParam);

            Hook.VK vk = (Hook.VK)key;

            switch (vk)
            {
                case Hook.VK.VK_F5:
                    MessageBox.Show("You pressed F5!");
                    break;

                case Hook.VK.VK_F6:
                    MessageBox.Show("You pressed F6!");
                    break;

                //etc...

                default: break;
            }
        }
    }



    [StructLayout(LayoutKind.Sequential)]
    public struct MOUSEHOOKSTRUCT
    {
        public POINT pt; // Can't use System.Windows.Point because that has X,Y as doubles, not integer
        public IntPtr hwnd;
        public uint wHitTestCode;
        public IntPtr dwExtraInfo;
        public override string ToString()
        {
            return $"({pt.X,4},{pt.Y,4})";
        }
    }

    public struct POINT
    {
        public int X;
        public int Y;
    }

    public enum HookType
    {
        WH_MIN = (-1),
        WH_MSGFILTER = (-1),
        WH_JOURNALRECORD = 0,
        WH_JOURNALPLAYBACK = 1,
        WH_KEYBOARD = 2,
        WH_GETMESSAGE = 3,
        WH_CALLWNDPROC = 4,
        WH_CBT = 5,
        WH_SYSMSGFILTER = 6,
        WH_MOUSE = 7,
        WH_HARDWARE = 8,
        WH_DEBUG = 9,
        WH_SHELL = 10,
        WH_FOREGROUNDIDLE = 11,
        WH_CALLWNDPROCRET = 12,
        WH_KEYBOARD_LL = 13,
        WH_MOUSE_LL = 14
    }
}
