using System;
using System.Runtime.InteropServices;

namespace Das.Views.Winforms
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BitMapInfo
    {
        //public BitMapInfo(int biSize)
        //{
        //    this.biSize = biSize;
        //}

        public int biSize;
        public int biWidth;
        public int biHeight;
        public short biPlanes;
        public short biBitCount;
        public int biCompression;
        public int biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public int biClrUsed;
        public int biClrImportant;
        public byte bmiColors_rgbBlue;
        public byte bmiColors_rgbGreen;
        public byte bmiColors_rgbRed;
        public byte bmiColors_rgbReserved;

        public void Init()
        {
            biSize = Marshal.SizeOf(this);
        }
    }
}
