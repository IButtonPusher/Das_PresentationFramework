using System;

namespace Das.Views.Styles
{
    [Flags]
    public enum StyleSelector
    {
        None = 1,
        Hover = 2,
        Active = 4,
        Checked = 8
    }
}
