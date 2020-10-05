using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.Views.Windows
{
    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Auto)]
    public class MONITORINFOEX
    {
        internal Int32 cbSize = Marshal.SizeOf(typeof(MONITORINFOEX));
        internal Int32 dwFlags;

        public RECT rcMonitor;
        internal RECT rcWork;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        internal Char[] szDevice = new Char[32];
    }
}