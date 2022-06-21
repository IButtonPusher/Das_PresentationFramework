using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Das.Views.Windows
{
    [StructLayout(LayoutKind.Explicit)]
    public class Pixelformatdescriptor
    {

       public const Byte PFD_TYPE_RGBA = 0;
       public const UInt32 PFD_DOUBLEBUFFER = 1;
       public const UInt32 PFD_DRAW_TO_WINDOW = 4;
       public const UInt32 PFD_SUPPORT_OPENGL = 32;
       public const SByte PFD_MAIN_PLANE = 0;

        public void Init()
        {
            nSize = (UInt16) Marshal.SizeOf(this);
        }

        [FieldOffset(27)] public Byte bReserved;

        [FieldOffset(22)] public Byte cAccumAlphaBits;

        [FieldOffset(18)] public Byte cAccumBits;

        [FieldOffset(21)] public Byte cAccumBlueBits;

        [FieldOffset(20)] public Byte cAccumGreenBits;

        [FieldOffset(19)] public Byte cAccumRedBits;

        [FieldOffset(16)] public Byte cAlphaBits;

        [FieldOffset(17)] public Byte cAlphaShift;

        [FieldOffset(25)] public Byte cAuxBuffers;

        [FieldOffset(14)] public Byte cBlueBits;

        [FieldOffset(15)] public Byte cBlueShift;

        [FieldOffset(9)] public Byte cColorBits;

        [FieldOffset(23)] public Byte cDepthBits;

        [FieldOffset(12)] public Byte cGreenBits;

        [FieldOffset(13)] public Byte cGreenShift;

        [FieldOffset(10)] public Byte cRedBits;

        [FieldOffset(11)] public Byte cRedShift;

        [FieldOffset(24)] public Byte cStencilBits;

        [FieldOffset(36)] public UInt32 dwDamageMask;

        [FieldOffset(4)] public UInt32 dwFlags;

        [FieldOffset(28)] public UInt32 dwLayerMask;

        [FieldOffset(32)] public UInt32 dwVisibleMask;

        [FieldOffset(26)] public SByte iLayerType;

        [FieldOffset(8)] public Byte iPixelType;

        [FieldOffset(0)] public UInt16 nSize;

        [FieldOffset(2)] public UInt16 nVersion;
    }
}