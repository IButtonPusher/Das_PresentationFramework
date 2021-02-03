using System;
using System.Drawing;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Views.Core.Geometry;
using Das.Views.Windows;
using Gdi.Shared;

namespace Das.Gdi
{
    public class GdiDevice : BaseGdiDevice
    {
        public GdiDevice(Color backgroundColor, 
                         ISize size)
            : base(backgroundColor, size)
        {
            var width = Math.Max(1, Convert.ToInt32(size.Width));
            var height = Math.Max(1, Convert.ToInt32(size.Height));
                
            
            _bmp = new Bitmap(width, height);
            
            _dcGraphics = _bmp.GetSmoothGraphics();
            
            _sizeLock = new Object();
            _backgroundColor = backgroundColor;
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
                using (var g = _bmp.GetSmoothGraphics())
                    g.Clear(_backgroundColor);

                return true;
            }
        }

        public Boolean UpdateSize(ISize size)
        {
            return UpdateSize(size.Width, size.Height);
        }

        private Bitmap _bmp;


        public void Clear()
        {
            lock (_sizeLock)
                _dcGraphics?.Clear(_backgroundColor);
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

                using (var g = _bmp.GetSmoothGraphics())
                {
                    var imgHdc = g.GetHdc();
                    Native.BitBlt(imgHdc, 0, 0, _width, _height, _memoryDeviceContext, 
                        0, 0, Native.SRCCOPY);
                    g.ReleaseHdc(imgHdc);
                }

                return new Bitmap(_bmp);
            }
        }

        private readonly Color _backgroundColor;
        private readonly Object _sizeLock;

        public override void Dispose()
        {
            base.Dispose();
            _bmp.Dispose();
        }
    }
}