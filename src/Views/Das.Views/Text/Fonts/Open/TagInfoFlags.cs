using System;
using System.Collections.Generic;
using System.Text;

namespace Das.Views.Text.Fonts
{
    [Flags]
    internal enum TagInfoFlags : uint
    {
        Substitution = 1,
        Positioning = 2,
        Both = Positioning | Substitution, // 0x00000003
        None = 0,
    }
}
