using System;
using System.Threading.Tasks;

namespace Das.Views.Text.Fonts
{
    [Flags]
    public enum GlyphFlags : ushort
    {
        Unassigned = 0,
        Base = 1,
        Ligature = 2,
        Mark = Ligature | Base, // 0x0003
        Component = 4,
        Unresolved = Component | Mark, // 0x0007
        GlyphTypeMask = Unresolved, // 0x0007
        Substituted = 16, // 0x0010
        Positioned = 32, // 0x0020
        NotChanged = 0,
        CursiveConnected = 64, // 0x0040
        ClusterStart = 256, // 0x0100
        Diacritic = 512, // 0x0200
        ZeroWidth = 1024, // 0x0400
        Missing = 2048, // 0x0800
        InvalidBase = 4096, // 0x1000
    }
}
