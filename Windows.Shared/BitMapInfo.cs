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

        public Int32 biSize;
        public Int32 biWidth;
        public Int32 biHeight;
        public Int16 biPlanes;
        public Int16 biBitCount;
        public Int32 biCompression;
        public Int32 biSizeImage;
        public Int32 biXPelsPerMeter;
        public Int32 biYPelsPerMeter;
        public Int32 biClrUsed;
        public Int32 biClrImportant;
        public Byte bmiColors_rgbBlue;
        public Byte bmiColors_rgbGreen;
        public Byte bmiColors_rgbRed;
        public Byte bmiColors_rgbReserved;

        public void Init()
        {
            biSize = Marshal.SizeOf(this);
        }
    }
}
