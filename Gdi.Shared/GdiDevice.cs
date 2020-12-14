using System;
using System.Drawing;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Views.Core.Geometry;
using Das.Views.Windows;

namespace Das.Gdi
{
    public class GdiDevice : BaseGdiDevice
    {
        public GdiDevice(Color backgroundColor, 
                         ISize size)
                         //Int32 width, 
                         //Int32 height)
            : base(backgroundColor, size)
        {
            //_memoryDeviceContext = IntPtr.Zero;
            //_currentDib = IntPtr.Zero;

            var width = Math.Max(1, Convert.ToInt32(size.Width));
            var height = Math.Max(1, Convert.ToInt32(size.Height));
                
            
            _bmp = new Bitmap(width, height);
            _dcGraphics = Graphics.FromImage(_bmp);
            
            _sizeLock = new Object();
            _backgroundColor = backgroundColor;
            //Width = width;
            //Height = height;
           // UpdateSize(size);
            //_lastRunWidth = 0;
            //_lastRunWidth = 0;
        }

        public Int32 Height => _height;

        public Int32 Width => _width;

        public Boolean UpdateSize(Double width,
                                  Double height)
        {
            lock (_sizeLock)
            {
                if (width.AreEqualEnough(_width) &&
                    height.AreEqualEnough(_height))
                    return false;

                if (width.IsZero() ||
                    height.IsZero())
                    return false;

                if (_currentDib != IntPtr.Zero)
                    Native.DeleteObject(_currentDib);
                
                _bmp.Dispose();
                _dcGraphics?.Dispose();
                
                if (_memoryDeviceContext != IntPtr.Zero)
                    DeleteDC(_memoryDeviceContext);
                
                var iwidth = Convert.ToInt32(width);
                var iheight = Convert.ToInt32(height);

                //////////////////////////////////
                if (!Invalidate(iwidth, iheight, _backgroundColor))
                    return false;
                //////////////////////////////////
                
                _bmp = new Bitmap(iwidth, iheight);
                using (var g = Graphics.FromImage(_bmp))
                    g.Clear(_backgroundColor);

                //var iwidth = Convert.ToInt32(width);
                //var iheight = Convert.ToInt32(height);

                //_memoryDeviceContext = CreateMemoryHdc(IntPtr.Zero, iwidth, iheight,
                //    out _currentDib);

                //if (_memoryDeviceContext == IntPtr.Zero) 
                //    return false;

                ////_bmp.Dispose();
                ////_dcGraphics.Dispose();
                //_bmp = new Bitmap(iwidth, iheight);
                //using (var g = Graphics.FromImage(_bmp))
                //    g.Clear(_backgroundColor);

                //_width = iwidth;
                //_height = iheight;

                //_dcGraphics = Graphics.FromHdc(_memoryDeviceContext);
                //_dcGraphics.Clear(_backgroundColor);

                //DumpDc();

                return true;
            }
        }

        public Boolean UpdateSize(ISize size)
        {
            return UpdateSize(size.Width, size.Height);
        }

        //private static IntPtr CreateMemoryHdc(IntPtr hdc, 
        //                                      Int32 width, 
        //                                      Int32 height,
        //                                      out IntPtr dib)
        //{
        //    var memoryHdc = Native.CreateCompatibleDC(hdc);
        //    SetBkMode(memoryHdc, 1);

        //    var info = new BitMapInfo();
        //    info.biSize = Marshal.SizeOf(info);
        //    info.biWidth = width;
        //    info.biHeight = -height;
        //    info.biPlanes = 1;
        //    info.biBitCount = 32;
        //    info.biCompression = 0; // BI_RGB
        //    dib = Native.CreateDIBSection(hdc, ref info, 0, out _, IntPtr.Zero, 0);
        //    Native.SelectObject(memoryHdc, dib);

        //    return memoryHdc;
        //}


        //[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        //public static extern Boolean DeleteDC(IntPtr hdc);

        private Bitmap _bmp;


        //private void DumpDc()
        //{
        //    //using (var test = new Bitmap(_width, _height, PixelFormat.Format32bppArgb))
        //    //{
        //    //    using (var g = Graphics.FromImage(test))
        //    //    {
        //    //        //g.Clear(Color.Transparent);

        //    //        var imgHdc = g.GetHdc();
        //    //        Native.BitBlt(imgHdc, 0, 0, _width, _height, _memoryDeviceContext, 
        //    //            0, 0, Native.SRCCOPY);
        //    //        g.ReleaseHdc(imgHdc);
        //    //    }

        //    //    test.Save("dump\\test_ " + _testCount++ + ".png");

                    
        //    //}
        //}

        public void Clear()
        {
            lock (_sizeLock)
                _dcGraphics?.Clear(_backgroundColor);
            
            //using (var g = Graphics.FromImage(_bmp))
            //    g.Clear(_backgroundColor);
        }
        
        public Bitmap? Run<TData>(TData data,
                                  Func<Graphics, TData, Boolean> action)
        {
            lock (_sizeLock)
            {
                if (_memoryDeviceContext == IntPtr.Zero || !(_dcGraphics is {} graphics))
                {
                    return new Bitmap(_bmp);
                }

                var rendered = action(graphics, data);
                if (!rendered)
                    return default;

                //DumpDc();

                using (var g = Graphics.FromImage(_bmp))
                {
                    var imgHdc = g.GetHdc();
                    Native.BitBlt(imgHdc, 0, 0, _width, _height, _memoryDeviceContext, 
                        0, 0, Native.SRCCOPY);
                    g.ReleaseHdc(imgHdc);
                }

                return new Bitmap(_bmp);
            }

            //action(_dcGraphics, data);

            //var width = Width;
            //var height = Height;
            //var bmp = _bmp;

            //if (width == 0 || height == 0)
            //{
            //    _empty ??= new Bitmap(1,1);
            //    return _empty;
            //}

            //var isNewBmp = false;

            //if (width != _lastRunWidth || 
            //    height != _lastRunHeight || 
            //    bmp == null)
            //{
            //    bmp?.Dispose();
            //    bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            //    using (var g = Graphics.FromImage(bmp))
            //        g.Clear(_backgroundColor);

            //    isNewBmp = true;
            //}

            //_lastRunWidth = width;
            //_lastRunHeight = height;

            

            //_testCount++;

            //return Run2(data, action, isNewBmp, bmp, width, height);
            
            //Debug.WriteLine("Drawing loop: " + _testCount);

            //var memoryHdc = CreateMemoryHdc(IntPtr.Zero, width, height, out var dib);
            //try
            //{
            //    using (var g = Graphics.FromHdc(memoryHdc))
            //    {
            //        if (isNewBmp)
            //        {
            //            Debug.WriteLine("clearing new image");
            //            g.Clear(_backgroundColor);
            //        }
            //        else
            //        {
            //            g.DrawImage(bmp, Point.Empty);
            //        }

            //        action(g, data);
            //    }

            //    using (var g = Graphics.FromImage(bmp))
            //    {
            //        var imgHdc = g.GetHdc();
            //        Native.BitBlt(imgHdc, 0, 0, width, height, memoryHdc, 0, 0, Native.SRCCOPY);
            //        g.ReleaseHdc(imgHdc);
            //    }
            //}
            //finally
            //{
            //    Native.DeleteObject(dib);
            //    DeleteDC(memoryHdc);
            //}

            //_bmp = bmp;
            
            //using (var g = Graphics.FromImage(bmp))
            //{
            //    //if (isNewBmp)
            //    //{
            //    //    //Debug.WriteLine("clearing new image");
            //    //    g.Clear(_backgroundColor);
            //    //}
            //    //else
            //    {
            //        //g.DrawImage(bmp, Point.Empty);
            //    }

            //    action(g, data);
            //}

            ////if (_testCount++ < 10)
            ////{
            ////    bmp.Save("test_ " + _testCount + ".png");
            ////    bmp.Save("res_ " + _testCount + ".png");
            ////}
            //var res = new Bitmap(bmp);
            //return res;
            
        }

        //private Int32 _testCount;


        //[DllImport("gdi32.dll")]
        //public static extern Int32 SetBkMode(IntPtr hdc, 
        //                                     Int32 mode);

        private readonly Color _backgroundColor;
        //private Bitmap? _empty;
        //private Int32 _lastRunWidth;
        //private Int32 _lastRunHeight;

        //private IntPtr _memoryDeviceContext;
        //private Graphics _dcGraphics;
        //private IntPtr _currentDib;
        private readonly Object _sizeLock;
        //private Int32 _width;
        //private Int32 _height;

        public override void Dispose()
        {
            base.Dispose();
            _bmp.Dispose();
           // _dcGraphics.Dispose();

        //    if (_currentDib != IntPtr.Zero)
        //        Native.DeleteObject(_currentDib);
                
        //    if (_memoryDeviceContext != IntPtr.Zero)
        //        DeleteDC(_memoryDeviceContext);
        }
    }
}