using Das.Views.Core.Geometry;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Das.Views.Windows;


namespace Das.Gdi
{
    public class BaseGdiDevice : IDisposable
    {
        public BaseGdiDevice(Color? backgroundColor, 
                             ISize size)
        {
            Invalidate(
                Convert.ToInt32(size.Width),
                Convert.ToInt32(size.Height), 
                backgroundColor);
        }

        private static IntPtr CreateMemoryHdc(IntPtr hdc, 
                                              Int32 width, 
                                              Int32 height,
                                              out IntPtr dib)
        {
            var memoryHdc = Native.CreateCompatibleDC(hdc);
            SetBkMode(memoryHdc, 1);

            var info = new BitMapInfo();
            info.biSize = Marshal.SizeOf(info);
            info.biWidth = width;
            info.biHeight = -height;
            info.biPlanes = 1;
            info.biBitCount = 32;
            info.biCompression = 0; // BI_RGB
            dib = Native.CreateDIBSection(hdc, ref info, 0, out _, IntPtr.Zero, 0);
            Native.SelectObject(memoryHdc, dib);

            return memoryHdc;
        }

        protected Boolean Invalidate(Int32 iwidth,
                                  Int32 iheight,
                                  Color? backgroundColor)
        {
            if (iwidth == 0 || iheight == 0)
                return false;

            _memoryDeviceContext = CreateMemoryHdc(IntPtr.Zero, iwidth, iheight,
                out _currentDib);

            if (_memoryDeviceContext == IntPtr.Zero) 
                return false;

            _width = iwidth;
            _height = iheight;

            _dcGraphics = Graphics.FromHdc(_memoryDeviceContext);
            
            if (backgroundColor != null)
                _dcGraphics.Clear(backgroundColor.Value);

            return true;
        }

        public Bitmap? ToBitmap(Color? backgroundColor)
        {
            if (_width== 0 || _height == 0)
                return default;
            
            var bmp = new Bitmap(_width, _height);
            using (var g = Graphics.FromImage(bmp))
            {
                if (backgroundColor != null)
                    g.Clear(backgroundColor.Value);
                
                var imgHdc = g.GetHdc();
                Native.BitBlt(imgHdc, 0, 0, _width, _height, _memoryDeviceContext,
                    0, 0, Native.SRCCOPY);
                g.ReleaseHdc(imgHdc);
            }

            return bmp;
        }


        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern Boolean DeleteDC(IntPtr hdc);
        
        [DllImport("gdi32.dll")]
        public static extern Int32 SetBkMode(IntPtr hdc, 
                                             Int32 mode);

        public virtual void Dispose()
        {
            _dcGraphics?.Dispose();

            if (_currentDib != IntPtr.Zero)
                Native.DeleteObject(_currentDib);
                
            if (_memoryDeviceContext != IntPtr.Zero)
                DeleteDC(_memoryDeviceContext);
        }

        public Graphics? Graphics => _dcGraphics;
        
        protected IntPtr _memoryDeviceContext;
        protected Graphics? _dcGraphics;
        protected IntPtr _currentDib;
        protected Int32 _width;
        protected Int32 _height;
        
    }
}
