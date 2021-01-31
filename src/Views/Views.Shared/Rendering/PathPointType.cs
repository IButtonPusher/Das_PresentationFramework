using System;

namespace Das.Views.Rendering
{
    /// <summary>
    /// https://referencesource.microsoft.com/#System.Drawing/commonui/System/Drawing/Advanced/PathPointType.cs,ab985741676c2f76
    /// </summary>
    public enum PathPointType : Byte
    {
        Start           = 0,    // move
        Line            = 1,    // line
        Bezier          = 3,    // default Beizer (= cubic Bezier)
        PathTypeMask    = 0x07, // type mask (lowest 3 bits).
        DashMode        = 0x10, // currently in dash mode.
        PathMarker      = 0x20, // a marker for the path.
        CloseSubpath    = 0x80, // closed flag
        Bezier3    = 3,    // cubic Bezier
    }
}
