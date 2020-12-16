using Das.Views.Core.Geometry;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Das.Extensions;
using Das.Views.Windows;


namespace Das.Gdi
{
    public class BaseGdiDevice : IDisposable
    {
        public BaseGdiDevice(Color backgroundColor, 
                             ISize size)
        {
            Invalidate(
                Convert.ToInt32(size.Width),
                Convert.ToInt32(size.Height), 
                backgroundColor);
        }
        
        protected static IntPtr CreateMemoryHdc(IntPtr hdc, 
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
                                  Color backgroundColor)
        {
            if (iwidth == 0 || iheight == 0)
                return false;
            
            //if (width.IsZero() || height.IsZero())
            //    return false;
            
            //var iwidth = Convert.ToInt32(width);
            //var iheight = Convert.ToInt32(height);

            _memoryDeviceContext = CreateMemoryHdc(IntPtr.Zero, iwidth, iheight,
                out _currentDib);

            if (_memoryDeviceContext == IntPtr.Zero) 
                return false;

            //_bmp.Dispose();
            //_dcGraphics.Dispose();
            //_bmp = new Bitmap(iwidth, iheight);
            //using (var g = Graphics.FromImage(_bmp))
            //    g.Clear(backgroundColor);

            _width = iwidth;
            _height = iheight;

            _dcGraphics = Graphics.FromHdc(_memoryDeviceContext);
            _dcGraphics.Clear(backgroundColor);

            return true;
        }

        public Bitmap? ToBitmap(Color backgroundColor)
        {
            if (_width== 0 || _height == 0)
                return default;
            
            var bmp = new Bitmap(_width, _height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(backgroundColor);
                
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
            //_bmp.Dispose();
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
        //private readonly Object _sizeLock;
        protected Int32 _width;
        protected Int32 _height;
        //private Bitmap _bmp;
    }
}
