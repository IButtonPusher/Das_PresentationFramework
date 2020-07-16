using Das.Views;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace MultiUiThreadedExample.CustomVisuals
{
    public class VisualTargetPresentationSource : HwndSource
        ,IDisposable
    {
        private readonly VisualTarget _visualTarget;

        public VisualTargetPresentationSource(HostVisual hostVisual, IntPtr parentWindow)
            : base(GetHwndSourceParameters())
        {
            var mainWindowSrc = HwndSource.FromHwnd(parentWindow);
            mainWindowSrc.AddHook(MainHook);
            _queue = new ConcurrentQueue<DispatcherOperationCallbackParameter>();

            _visualTarget = new VisualTarget(hostVisual);
            AddSource();

            var meth = typeof(HwndSource).GetMethod("InputFilterMessage",
                BindingFlags.NonPublic | BindingFlags.Instance);
            _methos = (Bob) Delegate.CreateDelegate(typeof(Bob), this, meth);
        }

        private Bob _methos;

        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll", EntryPoint = "WindowFromPoint", CharSet = CharSet.Auto)]
        private static extern IntPtr IntWindowFromPoint(POINT pt);

        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll", EntryPoint = "GetCursorPos", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool IntGetCursorPos([In, Out] POINTSTRUCT pt);

        [StructLayout(LayoutKind.Sequential)]
        public class POINTSTRUCT
        {
            public int x;
            public int y;

            public POINTSTRUCT()
            {
            }

            public POINTSTRUCT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }
        public void PumpQueueMessages()
        {
            while (_queue.TryDequeue(out var m))
            {
                var handled = m.handled;
                var original = m.hwnd;
                var me = Handle;

                var pt = new POINTSTRUCT();
                IntGetCursorPos(pt);

                var p = new POINT { X= pt.x, Y = pt.y};

                var num = IntWindowFromPoint(p);

                //0x03591706 - win32 sez
                //0x00441f8a - main window sez
                _methos(num, m.msg, m.wParam, m.lParam, ref handled);
            }

        }

        private delegate IntPtr Bob(IntPtr hwnd,
        int msg,
        IntPtr wParam,
        IntPtr lParam,
        ref bool handled);



        private class DispatcherOperationCallbackParameter
        {
            internal IntPtr hwnd;
            internal IntPtr wParam;
            internal IntPtr lParam;
            internal IntPtr retVal;
            internal int msg;
            internal bool handled;
        }

        private ConcurrentQueue<DispatcherOperationCallbackParameter> _queue;

        private IntPtr MainHook(IntPtr hwnd, Int32 msg, IntPtr wparam, IntPtr lparam, ref Boolean handled)
        {
            var callbackOperation = new DispatcherOperationCallbackParameter();
            
            
            callbackOperation.hwnd = hwnd;
            callbackOperation.msg = msg;
            callbackOperation.wParam = wparam;
            callbackOperation.lParam = lparam;

            _queue.Enqueue(callbackOperation);
            
            return IntPtr.Zero;
        }


        private static HwndSourceParameters GetHwndSourceParameters()
        {
            return new HwndSourceParameters
            { 
                WindowStyle = 0,
            };
        }

        private static IntPtr Target(IntPtr hwnd, Int32 msg, IntPtr wparam, IntPtr lparam, 
            ref Boolean handled)
        {
            System.Diagnostics.Debug.WriteLine("hwnd: " + msg);
            return IntPtr.Zero;
        }

        public override Visual RootVisual
        {
            get
            {
                try
                {
                    return _visualTarget?.RootVisual;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            set
            {
                var oldRoot = _visualTarget.RootVisual;

                // Set the root visual of the VisualTarget.  This visual will
                // now be used to visually compose the scene.
                _visualTarget.RootVisual = value;

                

                // Tell the PresentationSource that the root visual has
                // changed.  This kicks off a bunch of stuff like the
                // Loaded event.
                RootChanged(oldRoot, value);

                // Kickoff layout...
                var rootElement = value as UIElement;
                if (rootElement != null)
                {
                    rootElement.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    rootElement.Arrange(new Rect(rootElement.DesiredSize));
                }
            }
        }

        protected override CompositionTarget GetCompositionTargetCore()
        {
            return _visualTarget;
        }


        private bool _isDisposed;
        public override bool IsDisposed
        {
            get { return _isDisposed; }
        }

        public void Dispose()
        {
            RemoveSource();
            _isDisposed = true;
        }

        public void Unregister()
        {
            throw new NotImplementedException();
        }

        public Boolean OnNoMoreTabStops(TraversalRequest request)
        {
            throw new NotImplementedException();
        }

        public IKeyboardInputSink Sink => this;
    }
}