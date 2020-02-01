#if !HM3

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;

namespace Windows.Shared.Messages
{
    public class WindowsMessageRouter : IDisposable
    {
        public WindowsMessageRouter(IntPtr windowToHook)
        {
            
        }


        public void Dispose()
        {
            throw new NotImplementedException();
        }
        private bool _isTSFMessagePumpEnabled;

        [SecurityCritical]
        public bool GetMessage(ref MSG msg, IntPtr hwnd, int minMessage, int maxMessage)
        {
           // ITfMessagePump messagePump = this.GetMessagePump();
            try
            {
                //if (messagePump == null)
                    return GetMessageW(ref msg, new HandleRef((object)this, hwnd), minMessage, maxMessage);
//                int result;
//                messagePump.GetMessageW(ref msg, hwnd, minMessage, maxMessage, out result);
//                if (result == -1)
//                    throw new Win32Exception();
//                return result != 0;
            }
            finally
            {
//                if (messagePump != null)
//                    Marshal.ReleaseComObject((object)messagePump);
            }
        }

        [SecurityCritical]
        public static bool GetMessageW(
            [In, Out] ref MSG msg,
            HandleRef hWnd,
            int uMsgFilterMin,
            int uMsgFilterMax)
        {
            bool flag;
            switch (IntGetMessageW(ref msg, hWnd, uMsgFilterMin, uMsgFilterMax))
            {
                case -1:
                    throw new Win32Exception();
                case 0:
                    flag = false;
                    break;
                default:
                    flag = true;
                    break;
            }
            return flag;
        }

        //        [SecurityCritical]
        //        private ITfMessagePump GetMessagePump()
        //        {
        //            ITfMessagePump tfMessagePump = (ITfMessagePump)null;
        //            if (this._isTSFMessagePumpEnabled && Thread.CurrentThread.GetApartmentState() == ApartmentState.STA && TextServicesLoader.ServicesInstalled)
        //            {
        //                MS.Win32.UnsafeNativeMethods.ITfThreadMgr tfThreadMgr = TextServicesLoader.Load();
        //                if (tfThreadMgr != null)
        //                    tfMessagePump = tfThreadMgr as MS.Win32.UnsafeNativeMethods.ITfMessagePump;
        //            }
        //            return tfMessagePump;
        //        }

        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll", EntryPoint = "GetMessageW", CharSet = CharSet.Unicode, 
            SetLastError = true)]
        private static extern Int32 IntGetMessageW(
            [In, Out] ref MSG msg,
            HandleRef hWnd,
            Int32 uMsgFilterMin,
            Int32 uMsgFilterMax);
    }

    [SecurityCritical(SecurityCriticalScope.Everything)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("8f1b8ad8-0b6b-4874-90c5-bd76011e8f7c")]
    [SuppressUnmanagedCodeSecurity]
    [ComImport]
    internal interface ITfMessagePump
    {
        [SecurityCritical]
        void PeekMessageA(
            ref MSG msg,
            IntPtr hwnd,
            int msgFilterMin,
            int msgFilterMax,
            int removeMsg,
            out int result);

        [SecurityCritical]
        void GetMessageA(
            ref MSG msg,
            IntPtr hwnd,
            int msgFilterMin,
            int msgFilterMax,
            out int result);

        [SecurityCritical]
        void PeekMessageW(
            ref MSG msg,
            IntPtr hwnd,
            int msgFilterMin,
            int msgFilterMax,
            int removeMsg,
            out int result);

        [SecurityCritical]
        void GetMessageW(
            ref MSG msg,
            IntPtr hwnd,
            int msgFilterMin,
            int msgFilterMax,
            out int result);
    }
}
#endif