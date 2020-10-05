using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Das.Views.Core.Geometry
{
    [StructLayout(LayoutKind.Sequential)]
    public class Vector
    {
        public Int32 x;
        public Int32 y;
    }
}