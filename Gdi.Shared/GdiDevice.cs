using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Das.Views.Windows;

namespace Das.Gdi
{
    public class GdiDevice
    {
        public GdiDevice(Color backgroundColor, Int32 width, Int32 height)
        {
            _backgroundColor = backgroundColor;
            Width = width;
            Height = height;
        }

        public Int32 Height { get; set; }

        public Int32 Width { get; set; }


        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern Boolean DeleteDC(IntPtr hdc);

        public Bitmap Run<TData>(TData data, Action<Graphics, TData> action)
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
                    action(g, data);
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


        [DllImport("gdi32.dll")]
        public static extern Int32 SetBkMode(IntPtr hdc, Int32 mode);

        private static IntPtr CreateMemoryHdc(IntPtr hdc, Int32 width, Int32 height,
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

        private readonly Color _backgroundColor;
    }
}