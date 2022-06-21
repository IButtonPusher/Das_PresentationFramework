using System;
using System.Threading.Tasks;
using Das.Views.Windows;

namespace Das.OpenGL.Windows
{
   public class GLNativeWindowBuilder
   {
      static GLNativeWindowBuilder()
      {
         _wndProcDelegate = WndProc;
      }

      public GLNativeWindowBuilder(String nativeWindowClassName)
      {
         _nativeWindowClassName = nativeWindowClassName;
      }

      public IntPtr BuildNativeWindow(Int32 width,
                                      Int32 height)
      {
         var _ = GetGLWindowClass();
         var hwnd = Native.CreateWindowEx(0,
            _nativeWindowClassName,
            "",
            WindowStyles.WS_CLIPCHILDREN | WindowStyles.WS_CLIPSIBLINGS | WindowStyles.WS_POPUP,
            0, 0, width, height,
            IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

         return hwnd;
      }

      public WNDCLASSEX GetGLWindowClass()
      {
         var wndClass = new WNDCLASSEX();
         wndClass.Init();
         wndClass.style = ClassStyles.HorizontalRedraw | ClassStyles.VerticalRedraw
                                                       | ClassStyles.OwnDC;
         wndClass.lpfnWndProc = _wndProcDelegate;
         wndClass.cbClsExtra = 0;
         wndClass.cbWndExtra = 0;
         wndClass.hInstance = IntPtr.Zero;
         wndClass.hIcon = IntPtr.Zero;
         wndClass.hCursor = IntPtr.Zero;
         wndClass.hbrBackground = IntPtr.Zero;
         wndClass.lpszMenuName = null!;
         wndClass.lpszClassName = _nativeWindowClassName;
         wndClass.hIconSm = IntPtr.Zero;
         Native.RegisterClassEx(ref wndClass);

         return wndClass;
      }

      private static IntPtr WndProc(IntPtr hWnd,
                                    UInt32 msg,
                                    IntPtr wParam,
                                    IntPtr lParam)
      {
         return Native.DefWindowProc(hWnd, msg, wParam, lParam);
      }

      private static readonly UWndProc _wndProcDelegate;
      protected readonly String _nativeWindowClassName;
   }
}
