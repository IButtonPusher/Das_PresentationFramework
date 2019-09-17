using Das.Views.Winforms;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Das.Gdi
{
    public class GdiDevice
    {
        private readonly Color _backgroundColor;

        public Int32 Width { get; set; }
        public Int32 Height { get; set; }

        public GdiDevice(Color backgroundColor, Int32 width, Int32 height)
        {
            _backgroundColor = backgroundColor;
            Width = width;
            Height = height;
        }

        public Bitmap Run(Action<Graphics> action)
        {
            var width = Width;
            var height = Height;

            var image = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            var memoryHdc = CreateMemoryHdc(IntPtr.Zero, width, height, out var dib);
            try
            {
                using (var g = Graphics.FromHdc(memoryHdc))
                {
                    g.Clear(_backgroundColor);
                    action(g);
                }

                using (var g = Graphics.FromImage(image))
                {
                    var imgHdc = g.GetHdc();
                    Native.BitBlt(imgHdc, 0, 0, width, height, memoryHdc, 0, 0, Native.SRCCOPY);
                    g.ReleaseHdc(imgHdc);
                }
            }
            finally
            {
                Native.DeleteObject(dib);
                DeleteDC(memoryHdc);
            }

            return image;
        }

        private static IntPtr CreateMemoryHdc(IntPtr hdc, int width, int height,
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
      

        [DllImport("gdi32.dll")]
        public static extern int SetBkMode(IntPtr hdc, int mode);

        

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool DeleteDC(IntPtr hdc);
    }
}